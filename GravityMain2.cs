using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Gravity Main 2")]
public class GravityMain2 : Spell
{
	[HideInInspector]
	public float forceMultiplier = 1f;

	[HideInInspector]
	public float chargeMultiplier = 1f;

	private Vector3 currentTarget;

	private GameObject prefabInstance;

	private float damageModifier = 1f;

	[HideInInspector]
	public float AOEMod = 1f;

	[HideInInspector]
	public float stunChance;

	[HideInInspector]
	public bool doExtraImplosion;

	[HideInInspector]
	public bool moreDamagePerEnemy;

	[HideInInspector]
	public float doubleImplosionChance;

	[HideInInspector]
	public float tripleImplosionChance;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			AOEMod += 0.25f;
			break;
		case 2:
			stunChance += 10f * ((100f - stunChance) / 100f);
			break;
		case 3:
			doExtraImplosion = true;
			break;
		case 4:
			moreDamagePerEnemy = true;
			break;
		case 5:
			doubleImplosionChance += 20f * ((100f - doubleImplosionChance) / 100f);
			tripleImplosionChance += 2f * ((100f - tripleImplosionChance) / 100f);
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		currentTarget = targetPoint;
		if (prefabInstance == null)
		{
			prefabInstance = UnityEngine.Object.Instantiate(prefab, player.transform);
			Screenshake.Instance.AddTrauma(0.1f);
			prefabInstance.GetComponent<CircleCollider2D>().enabled = false;
			Screenshake.Instance.AddTrauma(0.35f);
		}
		Screenshake.Instance.AddTrauma(Time.deltaTime * 1.75f);
		chargeTimer += Time.deltaTime * chargeMultiplier;
		prefabInstance.transform.position = player.transform.position;
		float num = Mathf.Clamp(chargeTimer, 0.5f, 1f);
		prefabInstance.transform.localScale = new Vector3(num *= AOEMod, num *= AOEMod, 1f);
		if (chargeTimer >= 1f)
		{
			Charged(damageModifier);
		}
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		_ = currentTarget;
		PressurePiston component = prefabInstance.GetComponent<PressurePiston>();
		if ((float)new System.Random().Next(1, 100) < doubleImplosionChance)
		{
			component.extraImplosionsToDo = 1;
		}
		if ((float)new System.Random().Next(1, 100) < tripleImplosionChance)
		{
			component.extraImplosionsToDo = 2;
		}
		component.transform.parent = null;
		component.stunChance = stunChance;
		component.AOEDamageSize *= AOEMod;
		component.doExtraImplosion = doExtraImplosion;
		component.moreDamagePerEnemy = moreDamagePerEnemy;
		component.activated = true;
		prefabInstance.GetComponent<DamageOnEnter>();
		prefabInstance.GetComponent<CircleCollider2D>().enabled = true;
		component.setTarget(currentTarget, chargeTimer, damageMultiplier);
		Screenshake.Instance.AddTrauma(0.7f);
		base.Charged();
		prefabInstance = null;
	}
}
