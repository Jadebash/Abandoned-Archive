using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Water Special")]
public class WaterSpecial : SpecialSpell
{
	public GameObject rollPrefab;

	[HideInInspector]
	public float bubbleLength = 6f;

	[HideInInspector]
	public float rollDamage = 5f;

	[HideInInspector]
	public bool damageOnBurst;

	[HideInInspector]
	public float burstDamage;

	[HideInInspector]
	public bool increaseSpellDamage;

	[HideInInspector]
	public float spellDamageMultiplier;

	private Dictionary<GameObject, GameObject> bubbles;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			damageOnBurst = true;
			burstDamage += 5f;
			break;
		case 2:
			damage *= 3f;
			bubbleLength /= 2f;
			break;
		case 3:
			cooldownTime *= 0.9f;
			break;
		case 4:
			increaseSpellDamage = true;
			spellDamageMultiplier += 0.5f;
			bubbleLength /= 2f;
			break;
		case 5:
			rollDamage *= 1.5f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		bool flag = false;
		GameObject[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (Vector3.Distance(array2[i].transform.position, player.transform.position) < 8f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			player.GetComponent<SpellCasting>().SetSpellCooldownTime(this, cooldownTime);
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Fail");
		}
		else
		{
			CoroutineEmitter.Instance.StartCoroutine(Bubbles(player, damageMultiplier));
		}
	}

	public override void Roll(GameObject player)
	{
		CoroutineEmitter.Instance.StartCoroutine(RollSequence(player));
	}

	private IEnumerator RollSequence(GameObject player)
	{
		yield return new WaitForSeconds(0.5f);
		Object.Instantiate(rollPrefab, player.transform.position - new Vector3(0f, 0.34f, 0f), Quaternion.identity).GetComponent<DamageOverTime>().damagePerSecond = rollDamage;
	}

	private IEnumerator Bubbles(GameObject player, float damageMultiplier)
	{
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Water_Special", player.transform.position);
		bubbles = new Dictionary<GameObject, GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(gameObject.transform.position, player.transform.position) < 8f && !(gameObject.GetComponent<Health>() == null))
			{
				GameObject gameObject2 = Object.Instantiate(prefab, gameObject.transform.position, Quaternion.identity);
				Bubble component = gameObject2.GetComponent<Bubble>();
				component.target = gameObject.GetComponent<Health>();
				component.damagePerSecond = Mathf.RoundToInt(damage * damageMultiplier);
				component.tickLength = 1f;
				component.duration = bubbleLength;
				if (damageOnBurst)
				{
					component.burstDamage = burstDamage * damageMultiplier;
				}
				if (increaseSpellDamage)
				{
					component.damageModifierIncrease = spellDamageMultiplier;
				}
				bubbles.Add(gameObject, gameObject2);
			}
		}
		yield return new WaitForSeconds(bubbleLength);
		bubbles = new Dictionary<GameObject, GameObject>();
	}
}
