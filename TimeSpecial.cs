using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Time Special")]
public class TimeSpecial : SpecialSpell
{
	public float length = 5f;

	[HideInInspector]
	public float damageModifier;

	[HideInInspector]
	public float damageShareModifier;

	[HideInInspector]
	public bool chanceToDropHealth;

	[HideInInspector]
	public float knowledgeDropModifier;

	private GameObject player;

	private bool timeStopped;

	public GameObject timeEffect;

	private Animator timeStopAnim;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			length *= 1.3f;
			break;
		case 2:
			cooldownTime *= 0.9f;
			break;
		case 3:
			damageShareModifier += 0.2f;
			break;
		case 4:
			chanceToDropHealth = true;
			damage *= 0.8f;
			cooldownTime *= 1.2f;
			break;
		case 5:
			knowledgeDropModifier += 0.5f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		this.player = player;
		isCoroutineRunning = true;
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Time_Special");
		CoroutineEmitter.Instance.StartCoroutine(StopTime(length));
	}

	public override void Roll(GameObject player)
	{
		if (!timeStopped)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Time_Special_Roll");
			CoroutineEmitter.Instance.StartCoroutine(StopTime(0.2f));
		}
	}

	private IEnumerator StopTime(float time)
	{
		Enemy[] allEnemies = Object.FindObjectsOfType<Enemy>();
		Boss[] allBosses = Object.FindObjectsOfType<Boss>();
		Health[] array = Object.FindObjectsOfType<Health>();
		GameObject timeEffectInstance = Object.Instantiate(timeEffect);
		timeStopAnim = timeEffectInstance.GetComponent<Animator>();
		timeStopAnim.SetBool("isRolling", value: true);
		if (damageModifier != 0f)
		{
			player.GetComponent<SpellCasting>().spellDamageMultiplier += damageModifier;
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("Bullet");
		Dictionary<Rigidbody2D, Vector2> velocityDictionary = new Dictionary<Rigidbody2D, Vector2>();
		GameObject[] array3 = array2;
		for (int i = 0; i < array3.Length; i++)
		{
			Rigidbody2D component = array3[i].GetComponent<Rigidbody2D>();
			if (component != null)
			{
				velocityDictionary.Add(component, component.velocity);
				component.velocity = new Vector2(0f, 0f);
			}
		}
		Enemy[] array4 = allEnemies;
		foreach (Enemy enemy in array4)
		{
			enemy.Stun(time, doStunStars: false);
			enemy.GetComponent<Health>().OnDamage += TimeSpecial_OnDamage;
			enemy.knowledgeDropModifier += knowledgeDropModifier;
			if (enemy.spriteAnimator != null)
			{
				enemy.spriteAnimator.enabled = false;
			}
		}
		Boss[] array5 = allBosses;
		for (int i = 0; i < array5.Length; i++)
		{
			array5[i].GetComponent<Animator>().speed = 0.5f;
		}
		Dictionary<Health, float> regenDictionary = new Dictionary<Health, float>();
		Health[] array6 = array;
		foreach (Health health in array6)
		{
			if (health.regenRate != 0f)
			{
				regenDictionary.Add(health, health.regenRate);
				health.regenRate = 0f;
			}
		}
		timeStopped = true;
		yield return new WaitForSeconds(time);
		foreach (KeyValuePair<Rigidbody2D, Vector2> item in velocityDictionary)
		{
			if (item.Key != null)
			{
				item.Key.velocity = item.Value;
			}
		}
		array4 = allEnemies;
		foreach (Enemy enemy2 in array4)
		{
			if (enemy2 != null)
			{
				enemy2.GetComponent<Health>().OnDamage -= TimeSpecial_OnDamage;
				enemy2.knowledgeDropModifier -= knowledgeDropModifier;
				if (enemy2.spriteAnimator != null)
				{
					enemy2.spriteAnimator.enabled = true;
				}
			}
		}
		array5 = allBosses;
		for (int i = 0; i < array5.Length; i++)
		{
			array5[i].GetComponent<Animator>().speed = 1f;
		}
		foreach (KeyValuePair<Health, float> item2 in regenDictionary)
		{
			if (item2.Key != null)
			{
				item2.Key.regenRate = item2.Value;
			}
		}
		if (damageModifier != 0f)
		{
			player.GetComponent<SpellCasting>().spellDamageMultiplier -= damageModifier;
		}
		timeStopped = false;
		isCoroutineRunning = false;
		Object.Destroy(timeEffectInstance);
	}

	private void TimeSpecial_OnDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		if (attacker == null && damageShareModifier > 0f)
		{
			Enemy[] array = Object.FindObjectsOfType<Enemy>();
			foreach (Enemy enemy in array)
			{
				if (Vector3.Distance(enemy.transform.position, beingDamaged.transform.position) < 6f && enemy != beingDamaged)
				{
					enemy.GetComponent<Health>().Damage(Mathf.RoundToInt(damage * damageShareModifier), beingDamaged);
				}
			}
		}
		if (causedDeath && chanceToDropHealth && Random.Range(0f, 100f) < 10f)
		{
			RunManager.Instance.SpawnHealthPickup(beingDamaged.transform.position);
		}
	}
}
