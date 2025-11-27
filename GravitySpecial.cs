using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Gravity Special")]
public class GravitySpecial : SpecialSpell
{
	public GameObject gravityBurst;

	public GameObject gravityEffect;

	[HideInInspector]
	public float lengthModifier = 1f;

	[HideInInspector]
	public bool orbitingEnemySpellDamage;

	[HideInInspector]
	public float orbitingEnemySpellDamageMultiplier = 1f;

	[HideInInspector]
	public bool playerDamage;

	[HideInInspector]
	public float playerDamageMultiplier = 1f;

	[HideInInspector]
	public bool playerEvasion;

	[HideInInspector]
	public float playerEvasionChance;

	private EventInstance GravitySFX;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			cooldownTime *= 0.9f;
			break;
		case 2:
			orbitingEnemySpellDamage = true;
			orbitingEnemySpellDamageMultiplier += 0.5f;
			cooldownTime *= 1.25f;
			break;
		case 3:
			damage *= 1.5f;
			cooldownTime *= 0.75f;
			lengthModifier /= 2f;
			break;
		case 4:
			lengthModifier *= 1.5f;
			playerDamage = true;
			playerDamageMultiplier = 2f;
			break;
		case 5:
			playerEvasionChance += 0.25f;
			playerEvasion = true;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		isCoroutineRunning = true;
		CoroutineEmitter.Instance.StartCoroutine(Orbit(player.GetComponent<Movement>(), damageMultiplier));
		GravitySFX = RuntimeManager.CreateInstance("event:/SFX/Main Character/Spells/Gravity_Special");
		GravitySFX.start();
	}

	public override void Roll(GameObject player)
	{
		CoroutineEmitter.Instance.StartCoroutine(AdjustSpeed(player.GetComponent<Movement>()));
	}

	private IEnumerator AdjustSpeed(Movement playerMovement)
	{
		playerMovement.AddSpeedEffect(0.3f, 0.6f, "GravitySpecial");
		yield return new WaitForSeconds(0.1f);
		Screenshake.Instance.AddTrauma(0.2f);
		Object.Instantiate(gravityBurst, playerMovement.transform.position, Quaternion.identity);
		playerMovement.anim.GetComponent<SpriteRenderer>().enabled = false;
		yield return new WaitForSeconds(0.4f);
		playerMovement.anim.GetComponent<SpriteRenderer>().enabled = true;
		yield return new WaitForSeconds(0.1f);
	}

	private IEnumerator Orbit(Movement playerMovement, float damageMultiplier = 1f)
	{
		Transform player = playerMovement.transform;
		GameObject gravityEffectInstance = Object.Instantiate(gravityEffect, player);
		Volume gravityVisualEffect = gravityEffectInstance.transform.GetChild(0).GetComponent<Volume>();
		gravityVisualEffect.weight = 0f;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		List<GameObject> particleEffects = new List<GameObject>();
		List<Rigidbody2D> enemies = new List<Rigidbody2D>();
		float originalDamageModifier = 1f;
		if (playerDamage)
		{
			Health component = playerMovement.GetComponent<Health>();
			originalDamageModifier = component.damageModifier;
			component.damageModifier *= playerDamageMultiplier;
		}
		if (playerEvasion)
		{
			playerMovement.GetComponent<Health>().evasionChance += playerEvasionChance;
		}
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!(gameObject.GetComponent<Orbite>() != null) && Vector3.Distance(gameObject.transform.position, player.position) < 5f && !enemies.Contains(gameObject.GetComponent<Rigidbody2D>()))
			{
				if (orbitingEnemySpellDamage)
				{
					gameObject.GetComponent<Health>().damageModifier = orbitingEnemySpellDamageMultiplier;
				}
				if ((bool)gameObject.GetComponent<Enemy>())
				{
					gameObject.GetComponent<Enemy>().Stun(4.5f * lengthModifier);
				}
				if ((bool)gameObject.GetComponent<Rigidbody2D>())
				{
					enemies.Add(gameObject.GetComponent<Rigidbody2D>());
				}
				particleEffects.Add(Object.Instantiate(prefab, gameObject.transform));
			}
		}
		for (int j = 0; j < Mathf.RoundToInt(60f * lengthModifier); j++)
		{
			array2 = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject2 in array2)
			{
				if (gameObject2.GetComponent<Orbite>() != null || !(Vector3.Distance(gameObject2.transform.position, player.position) < 3f) || enemies.Contains(gameObject2.GetComponent<Rigidbody2D>()))
				{
					continue;
				}
				if ((bool)gameObject2.GetComponent<Rigidbody2D>())
				{
					enemies.Add(gameObject2.GetComponent<Rigidbody2D>());
					if ((bool)gameObject2.GetComponent<Enemy>())
					{
						gameObject2.GetComponent<Enemy>().Stun(Mathf.Max(4.5f * lengthModifier - (float)j * 0.1f, 0f));
					}
				}
				particleEffects.Add(Object.Instantiate(prefab, gameObject2.transform));
				if (j % 8 == 0)
				{
					gameObject2.GetComponent<Health>().Damage(Mathf.Round(damage * damageMultiplier));
				}
			}
			Screenshake.Instance.AddTrauma(0.2f);
			foreach (Rigidbody2D item in enemies)
			{
				if (item != null)
				{
					Vector3 lhs = item.transform.position - player.position;
					Vector3 normalized = Vector3.Cross(lhs, Vector3.forward).normalized;
					item.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
					item.AddForce(new Vector2(normalized.x, normalized.y) * -440f);
					item.AddForce(new Vector2(lhs.x, lhs.y) * -88f);
					if (j % 8 == 0)
					{
						item.GetComponent<Health>()?.Damage(Mathf.Round(damage * damageMultiplier));
					}
				}
			}
			if (j < 30)
			{
				gravityVisualEffect.weight = Mathf.Clamp01((float)j / 5f);
			}
			else
			{
				gravityVisualEffect.weight = Mathf.Clamp01((60f - (float)j) / 5f);
			}
			yield return new WaitForSeconds(0.1f);
			if (j == 30)
			{
				gravityEffectInstance.GetComponent<ParticleSystem>().Stop();
			}
		}
		foreach (Rigidbody2D item2 in enemies)
		{
			if (item2 != null)
			{
				if (orbitingEnemySpellDamage)
				{
					item2.GetComponent<Health>().damageModifier = 1f;
				}
				Vector3 vector = item2.transform.position - player.position;
				Enemy component2 = item2.GetComponent<Enemy>();
				if (component2 != null)
				{
					component2.Stun(0.5f);
				}
				item2.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
				item2.AddForce(new Vector2(vector.x, vector.y) * 40f, ForceMode2D.Impulse);
				item2.velocity = Vector2.ClampMagnitude(item2.velocity, 25f);
			}
		}
		gravityVisualEffect.weight = 0f;
		foreach (GameObject item3 in particleEffects)
		{
			if (item3 != null)
			{
				Object.Destroy(item3);
			}
		}
		if (playerDamage)
		{
			playerMovement.GetComponent<Health>().damageModifier = originalDamageModifier;
		}
		if (playerEvasion)
		{
			playerMovement.GetComponent<Health>().evasionChance -= playerEvasionChance;
		}
		GravitySFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		isCoroutineRunning = false;
		yield return new WaitForSeconds(3f * lengthModifier);
		Object.Destroy(gravityEffectInstance);
	}
}
