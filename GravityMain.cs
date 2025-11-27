using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Gravity Main")]
public class GravityMain : Spell
{
	[HideInInspector]
	public float forceMultiplier = 1f;

	[HideInInspector]
	public float pushMaxSpeed = 25f;

	[HideInInspector]
	public bool aoe;

	[HideInInspector]
	public float aoeRadius;

	[HideInInspector]
	public bool pushAllChance;

	[HideInInspector]
	public float pushAllRandomChance;

	[HideInInspector]
	public float enemyWallDamageModifier = 1f;

	[HideInInspector]
	public int upgrade1Count;

	[HideInInspector]
	public bool hasUpgrade1;

	[HideInInspector]
	public float baselineCooldownTime;

	[HideInInspector]
	public float accumulatedCooldownBonus;

	[HideInInspector]
	public int castCount;

	[HideInInspector]
	public int upgrade3Count;

	[HideInInspector]
	public bool hasUpgrade3;

	private static Dictionary<GameObject, (bool hitWall, bool isDead, int upgradeCount)> pushedEnemies = new Dictionary<GameObject, (bool, bool, int)>();

	private void EnsureBaselineInitialized()
	{
		if (baselineCooldownTime == 0f && cooldownTime > 0f)
		{
			baselineCooldownTime = cooldownTime;
		}
	}

	private void UpdateCooldownTime()
	{
		EnsureBaselineInitialized();
		cooldownTime = baselineCooldownTime + accumulatedCooldownBonus;
	}

	private void ProcessCooldownBonus()
	{
		if (hasUpgrade3)
		{
			castCount++;
			float b = 10f * (float)upgrade3Count;
			accumulatedCooldownBonus = Mathf.Min(accumulatedCooldownBonus + (float)upgrade3Count, b);
			if (castCount >= 10)
			{
				accumulatedCooldownBonus = 0f;
				castCount = 0;
			}
			UpdateCooldownTime();
		}
	}

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			forceMultiplier += 0.25f;
			upgrade1Count++;
			hasUpgrade1 = true;
			break;
		case 2:
			damage *= 1.4f;
			EnsureBaselineInitialized();
			baselineCooldownTime *= 1.2f;
			UpdateCooldownTime();
			break;
		case 3:
			upgrade3Count++;
			hasUpgrade3 = true;
			EnsureBaselineInitialized();
			UpdateCooldownTime();
			break;
		case 4:
			aoe = true;
			target = TargetType.Point;
			aoeRadius = 1f;
			break;
		case 5:
			pushAllChance = true;
			pushAllRandomChance += 25f;
			pushAllRandomChance = Mathf.Min(pushAllRandomChance, 100f);
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		List<GameObject> list = new List<GameObject>();
		if (pushAllChance && Random.Range(0f, 100f) < pushAllRandomChance)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject in array)
			{
				if (Vector2.Distance(gameObject.transform.position, player.transform.position) < 10f)
				{
					list.Add(gameObject);
				}
			}
		}
		else if (!aoe)
		{
			list.Add(target);
		}
		else
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject2 in array)
			{
				if (Vector2.Distance(gameObject2.transform.position, targetPoint) < aoeRadius)
				{
					list.Add(gameObject2);
				}
			}
		}
		if (list.Count > 0)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Gravity_Main", player.transform.position);
			CastAnimationOneHanded(player);
			Screenshake.Instance.AddTrauma(0.6f);
		}
		float num = (hasUpgrade3 ? (accumulatedCooldownBonus / 6.4f + 1.2f) : 1f);
		foreach (GameObject item2 in list)
		{
			Object.Instantiate(prefab, item2.transform.position, Quaternion.identity);
			Health component = item2.GetComponent<Health>();
			component.Damage(Mathf.Round(damage * damageMultiplier * num));
			bool item = component.health <= 0f;
			Rigidbody2D component2 = item2.GetComponent<Rigidbody2D>();
			if (!(component2 != null))
			{
				continue;
			}
			component2.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			Enemy component3 = item2.GetComponent<Enemy>();
			if (component3 != null)
			{
				component3.Stun(0.5f);
				component3.wallCollisionDamageMultiplier = enemyWallDamageModifier;
			}
			component2.AddForce(new Vector2(player.transform.position.x - item2.transform.position.x, player.transform.position.y - item2.transform.position.y).normalized * -60f * forceMultiplier, ForceMode2D.Impulse);
			component2.velocity = Vector2.ClampMagnitude(component2.velocity, pushMaxSpeed);
			if (hasUpgrade1)
			{
				pushedEnemies[item2] = (false, item, upgrade1Count);
				if (CoroutineEmitter.Instance != null)
				{
					CoroutineEmitter.Instance.StartCoroutine(DelayedDamageCheck(item2));
				}
			}
		}
		if (hasUpgrade3 && list.Count > 0)
		{
			ProcessCooldownBonus();
		}
	}

	private IEnumerator DelayedDamageCheck(GameObject enemy)
	{
		yield return new WaitForSeconds(1f);
		if (enemy == null || !pushedEnemies.TryGetValue(enemy, out (bool, bool, int) value))
		{
			yield break;
		}
		if (!value.Item1 && !value.Item2)
		{
			Health component = enemy.GetComponent<Health>();
			if (component != null && component.health > 0f)
			{
				component.Damage(value.Item3);
			}
		}
		pushedEnemies.Remove(enemy);
	}

	public static void OnGravityPushedEnemyHitWall(GameObject enemy)
	{
		if (enemy == null || !pushedEnemies.TryGetValue(enemy, out (bool, bool, int) value) || value.Item1)
		{
			return;
		}
		Rigidbody2D component = enemy.GetComponent<Rigidbody2D>();
		if (component != null)
		{
			component.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
		}
		Health component2 = enemy.GetComponent<Health>();
		if (component2 != null && component2.health > 0f)
		{
			if (Random.Range(0f, 100f) < 25f)
			{
				Enemy component3 = enemy.GetComponent<Enemy>();
				if (component3?.knowledgePelletPrefab != null && Object.Instantiate(component3.knowledgePelletPrefab, enemy.transform.position + new Vector3(Random.insideUnitCircle.x * 0.5f, Random.insideUnitCircle.y * 0.5f, 0f), Quaternion.identity).TryGetComponent<KnowledgePellet>(out var component4))
				{
					component4.amount = 1;
				}
			}
			pushedEnemies[enemy] = (true, false, value.Item3);
		}
		else
		{
			pushedEnemies[enemy] = (false, true, value.Item3);
		}
	}

	public static void OnGravityPushedEnemyDied(GameObject enemy)
	{
		if (enemy != null && pushedEnemies.TryGetValue(enemy, out (bool, bool, int) value))
		{
			pushedEnemies[enemy] = (value.Item1, true, value.Item3);
		}
	}

	public static bool IsGravityPushedEnemy(GameObject enemy)
	{
		if (enemy != null)
		{
			return pushedEnemies.ContainsKey(enemy);
		}
		return false;
	}
}
