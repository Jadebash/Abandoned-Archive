using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Air Main")]
public class AirMain : Spell
{
	public GameObject comboPrefab;

	public GameObject bigPrefab;

	public GameObject bigComboPrefab;

	public float comboDamageMultiplier = 2f;

	private GameObject player;

	[HideInInspector]
	public bool useBigAOE;

	[HideInInspector]
	public bool reduceCooldownOnComboHit;

	[HideInInspector]
	public float cooldownSpeedChangeModifier;

	[HideInInspector]
	public float specialSpellCooldownReduction;

	[HideInInspector]
	public bool increaseMovementSpeedOnComboHit;

	[HideInInspector]
	public float speedBuffDuration;

	[HideInInspector]
	public float timeBetween;

	private bool cooldownReductionTriggered;

	private bool movementSpeedTriggered;

	private bool specialSpellReductionTriggered;

	[HideInInspector]
	public float internalCooldownTime = 0.5f;

	[HideInInspector]
	public float baseCooldownTime;

	[HideInInspector]
	public bool initialized;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			comboDamageMultiplier *= 1.5f;
			break;
		case 2:
			reduceCooldownOnComboHit = true;
			cooldownSpeedChangeModifier += 0.25f;
			internalCooldownTime += 1f;
			break;
		case 3:
			useBigAOE = true;
			break;
		case 4:
			specialSpellCooldownReduction += 0.15f;
			break;
		case 5:
			increaseMovementSpeedOnComboHit = true;
			speedBuffDuration += 1f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		if (!initialized)
		{
			baseCooldownTime = cooldownTime;
			initialized = true;
		}
		Screenshake.Instance.AddTrauma(0.8f);
		this.player = player;
		if (comboCounter == 1)
		{
			timeBetween = Time.time;
		}
		else
		{
			if (Time.time - timeBetween > 1f)
			{
				comboCounter = 1;
			}
			timeBetween = Time.time;
		}
		if (comboCounter == 3)
		{
			cooldownTime = baseCooldownTime + internalCooldownTime;
		}
		else
		{
			cooldownTime = baseCooldownTime;
		}
		if (comboCounter == 3)
		{
			comboCounter = 0;
			cooldownReductionTriggered = false;
			movementSpeedTriggered = false;
			specialSpellReductionTriggered = false;
			GameObject gameObject = ((!useBigAOE) ? Object.Instantiate(comboPrefab, player.transform.position, Quaternion.identity) : Object.Instantiate(bigComboPrefab, player.transform.position, Quaternion.identity));
			gameObject.GetComponent<DamageOnEnter>().damage = Mathf.Round(damage * comboDamageMultiplier * damageMultiplier);
			DamageOnEnter component = gameObject.GetComponent<DamageOnEnter>();
			if (reduceCooldownOnComboHit)
			{
				component.OnHit += SuccessfulComboHit;
			}
			if (increaseMovementSpeedOnComboHit)
			{
				component.OnHit += OnComboHitSpeed;
			}
			if (specialSpellCooldownReduction > 0f)
			{
				component.OnHit += OnAttackHit;
			}
		}
		else
		{
			GameObject gameObject = Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
			gameObject.GetComponent<DamageOnEnter>().damage = Mathf.Round(damage * damageMultiplier);
			Vector3 vector = targetPoint - player.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num - 90f);
			CastAnimationOneHanded(player);
			specialSpellReductionTriggered = false;
			DamageOnEnter component2 = gameObject.GetComponent<DamageOnEnter>();
			if (specialSpellCooldownReduction > 0f)
			{
				component2.OnHit += OnAttackHit;
			}
		}
	}

	public void SuccessfulComboHit()
	{
		if (reduceCooldownOnComboHit && !cooldownReductionTriggered)
		{
			cooldownReductionTriggered = true;
			CoroutineEmitter.Instance.StartCoroutine(ReduceCooldowns());
		}
	}

	private IEnumerator ReduceCooldowns()
	{
		player.GetComponent<SpellCasting>().cooldownSpeedModifier += cooldownSpeedChangeModifier;
		yield return new WaitForSeconds(2f);
		player.GetComponent<SpellCasting>().cooldownSpeedModifier -= cooldownSpeedChangeModifier;
	}

	public void OnComboHitSpeed()
	{
		if (increaseMovementSpeedOnComboHit && speedBuffDuration > 0f && !movementSpeedTriggered)
		{
			movementSpeedTriggered = true;
			CoroutineEmitter.Instance.StartCoroutine(IncreaseSpeed());
		}
	}

	private IEnumerator IncreaseSpeed()
	{
		player.GetComponent<Movement>().AddSpeedEffect(0.2f, speedBuffDuration, "AirMain");
		yield return new WaitForSeconds(speedBuffDuration);
	}

	public void OnAttackHit()
	{
		if (specialSpellCooldownReduction > 0f && !specialSpellReductionTriggered)
		{
			specialSpellReductionTriggered = true;
			SpellCasting component = player.GetComponent<SpellCasting>();
			if (component != null)
			{
				component.ReduceSpecialSpellCooldown(specialSpellCooldownReduction);
			}
		}
	}
}
