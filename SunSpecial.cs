using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Sun Special")]
public class SunSpecial : SpecialSpell
{
	[HideInInspector]
	public float scale = 1f;

	[HideInInspector]
	public float movementSpeedBuff = 0.3f;

	[HideInInspector]
	public bool chanceToHeal;

	[HideInInspector]
	public float healChance;

	private static int lastHealSceneIndex = -1;

	private static int healCount = 0;

	[HideInInspector]
	public int maxHeals = 1;

	public GameObject sunEffect;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			movementSpeedBuff *= 1.2f;
			cooldownTime *= 1.2f;
			break;
		case 2:
			cooldownTime *= 0.9f;
			break;
		case 3:
			scale *= 1.25f;
			movementSpeedBuff *= 0.8f;
			break;
		case 4:
			chanceToHeal = true;
			healChance += 10f;
			maxHeals++;
			break;
		case 5:
			damage *= 1.75f;
			scale *= 0.8f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		isCoroutineRunning = true;
		CoroutineEmitter.Instance.StartCoroutine(BlessingOfTheSun(player.GetComponent<Movement>(), damageMultiplier, player.GetComponent<SpellCasting>()));
	}

	public override void Roll(GameObject player)
	{
		Object.Instantiate(sunEffect, player.transform.position, Quaternion.identity, player.transform);
		Enemy[] array = Object.FindObjectsOfType<Enemy>();
		foreach (Enemy enemy in array)
		{
			if (Vector3.Distance(enemy.transform.position, player.transform.position) < 1.8f)
			{
				Rigidbody2D component = enemy.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					enemy.Stun(0.5f);
					Vector2 vector = (enemy.transform.position - player.transform.position).normalized;
					component.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
					component.AddForce(vector * 30f, ForceMode2D.Impulse);
					component.velocity = Vector2.ClampMagnitude(component.velocity, 15f);
				}
			}
		}
	}

	private IEnumerator BlessingOfTheSun(Movement playerMovement, float damageMultiplier, SpellCasting spellCasting)
	{
		if (prefab == null)
		{
			Debug.LogError("SunSpecial prefab is missing!");
			isCoroutineRunning = false;
			yield break;
		}
		if (playerMovement == null)
		{
			isCoroutineRunning = false;
			yield break;
		}
		GameObject gameObject = Object.Instantiate(prefab, playerMovement.transform.position, Quaternion.identity);
		gameObject.transform.localScale = new Vector3(scale, scale, 1f);
		DamageOverTime component = gameObject.GetComponent<DamageOverTime>();
		Follow component2 = gameObject.GetComponent<Follow>();
		Animator animator = gameObject.GetComponent<Animator>();
		RuntimeManager.StudioSystem.getParameterByName("Intensity", out var value);
		if (chanceToHeal && value > 0f)
		{
			int buildIndex = SceneManager.GetActiveScene().buildIndex;
			if (buildIndex != lastHealSceneIndex)
			{
				healCount = 0;
				lastHealSceneIndex = buildIndex;
			}
			if (healCount < maxHeals)
			{
				Debug.Log("Healing");
				if (Random.Range(0f, 100f) < healChance)
				{
					Health component3 = playerMovement.GetComponent<Health>();
					if (component3 != null)
					{
						component3.Heal(20f);
						healCount++;
					}
				}
			}
			else
			{
				Debug.Log("Max heals reached for this floor");
			}
		}
		if (component != null)
		{
			component.damagePerSecond = damage * damageMultiplier;
		}
		if (component2 != null)
		{
			component2.target = playerMovement.transform;
		}
		if (playerMovement != null)
		{
			playerMovement.sunSpecialModifier = movementSpeedBuff;
		}
		yield return new WaitForSeconds(6f);
		if (playerMovement != null)
		{
			playerMovement.sunSpecialModifier = 0f;
		}
		if (animator != null)
		{
			animator.SetTrigger("stop");
		}
		isCoroutineRunning = false;
	}
}
