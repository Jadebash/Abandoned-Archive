using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Lightning Special")]
public class LightningSpecial : SpecialSpell
{
	private Vector2 targetOriginalPosition;

	private Transform targetOriginalParent;

	public GameObject sparksPrefab;

	public GameObject rollSparksPrefab;

	public GameObject lightningRollPrefab;

	private GameObject player;

	[HideInInspector]
	public int timesToStrike = 1;

	[HideInInspector]
	public bool damageSingleEnemy;

	[HideInInspector]
	public bool reduceCooldownOnKill;

	[HideInInspector]
	public bool stunEnemies;

	[HideInInspector]
	public float stunEnemiesTime;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			timesToStrike++;
			damage *= 0.75f;
			break;
		case 2:
			damage *= 3f;
			cooldownTime *= 2f;
			damageSingleEnemy = true;
			break;
		case 3:
			reduceCooldownOnKill = true;
			break;
		case 4:
			cooldownTime *= 0.75f;
			damage *= 0.9f;
			break;
		case 5:
			stunEnemies = true;
			stunEnemiesTime += 1.5f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		this.player = player;
		if (target == null)
		{
			player.GetComponent<SpellCasting>().SetSpellCooldownTime(this, cooldownTime);
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Fail");
			return;
		}
		if ((bool)target.GetComponent<Enemy>())
		{
			target.GetComponent<Enemy>().Stun(1.3f);
		}
		Object.Instantiate(sparksPrefab, target.transform);
		CoroutineEmitter.Instance.StartCoroutine(LightningStrike(target, damageMultiplier));
	}

	private IEnumerator LightningStrike(GameObject target, float damageMultiplier)
	{
		for (int i = 0; i < timesToStrike; i++)
		{
			yield return new WaitForSeconds(1f);
			Strike(target, damageMultiplier);
		}
	}

	public void Strike(GameObject target, float damageMultiplier)
	{
		if (!(target != null))
		{
			return;
		}
		Object.Instantiate(prefab, target.transform.position, Quaternion.identity);
		bool flag = false;
		if (damageSingleEnemy)
		{
			if ((bool)target.GetComponent<Health>())
			{
				flag = target.GetComponent<Health>().Damage(Mathf.Round(damage * damageMultiplier));
			}
		}
		else
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject in array)
			{
				if (Vector2.Distance(gameObject.transform.position, target.transform.position) < 2.5f)
				{
					if (gameObject.GetComponent<Health>().Damage(Mathf.Round(damage * damageMultiplier)) && (bool)gameObject.GetComponent<Enemy>())
					{
						flag = true;
					}
					if (stunEnemies && (bool)gameObject.GetComponent<Enemy>())
					{
						gameObject.GetComponent<Enemy>().Stun(stunEnemiesTime);
					}
				}
			}
		}
		if (!(reduceCooldownOnKill && flag))
		{
			return;
		}
		SpellCasting component = player.GetComponent<SpellCasting>();
		float num = 1f;
		foreach (Stance stance in component.stances)
		{
			if (stance.spellSlots.Count > 0 && stance.spellSlots[0].spell == this)
			{
				SpellSlot spellSlot = stance.spellSlots[0];
				if (spellSlot.spell.cooldownTime > 0f)
				{
					num = spellSlot.cooldownTimer / spellSlot.spell.cooldownTime;
				}
				break;
			}
		}
		if (num < 0.75f)
		{
			component.SetSpellCooldownTime(this, cooldownTime * 0.75f);
		}
	}

	public override void Roll(GameObject player)
	{
		CoroutineEmitter.Instance.StartCoroutine(RollZap(player));
		Object.Instantiate(rollSparksPrefab, player.transform);
	}

	private IEnumerator RollZap(GameObject player)
	{
		yield return new WaitForSeconds(0.2f);
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		List<GameObject> list = new List<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject item in array2)
		{
			list.Add(item);
		}
		GameObject gameObject = null;
		float num = 3f;
		foreach (GameObject item2 in list)
		{
			float num2 = Vector3.Distance(item2.transform.position, player.transform.position);
			if (num2 < num)
			{
				gameObject = item2.gameObject;
				num = num2;
			}
		}
		if (gameObject != null)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Lightning_Main", gameObject.transform.position);
			GameObject gameObject2 = Object.Instantiate(lightningRollPrefab);
			gameObject2.GetComponent<TetheringLine>().from = player.transform;
			gameObject2.GetComponent<TetheringLine>().to = gameObject.transform;
			gameObject2.GetComponent<TetheringLine>().damagePerSecond = 3f;
		}
	}
}
