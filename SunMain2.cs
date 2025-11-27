using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Sun Main 2")]
public class SunMain2 : Spell
{
	private GameObject prefabInstance;

	private Vector3 currentTarget;

	private float forceMultiplier = 10f;

	private float lifeSpan = 3f;

	private float AOERange = 3f;

	private float largeRockChance;

	private float dazeChance;

	private bool canBounce;

	private float bounceDamageMod;

	private float bounceRangeMod;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			forceMultiplier *= 1.5f;
			break;
		case 2:
			dazeChance += 20f * ((100f - dazeChance) / 100f);
			break;
		case 3:
			largeRockChance += 5f * ((100f - largeRockChance) / 100f);
			damage *= 2f;
			break;
		case 4:
			cooldownTime *= 0.8f;
			break;
		case 5:
			canBounce = true;
			bounceDamageMod += 0.1f;
			bounceRangeMod += 0.1f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		if (prefabInstance == null)
		{
			prefabInstance = UnityEngine.Object.Instantiate(prefab, player.transform.position + (targetPoint - player.transform.position).normalized * 0.25f, Quaternion.identity);
		}
		bool doDaze = false;
		if ((float)new System.Random().Next(1, 100) < dazeChance)
		{
			doDaze = true;
		}
		SolarPebble component = prefabInstance.GetComponent<SolarPebble>();
		if ((float)new System.Random().Next(1, 100) < largeRockChance)
		{
			prefabInstance.transform.localScale = new Vector3(2f, 2f, 1f);
		}
		component.lifeSpan = lifeSpan;
		component.movementForce = forceMultiplier;
		component.AOEDamage = damage;
		component.AOERange = AOERange;
		component.doDaze = doDaze;
		component.canBounce = canBounce;
		component.bounceRangeMod = bounceRangeMod;
		component.bounceDamageMod = bounceDamageMod;
		prefabInstance.GetComponent<Rigidbody2D>().AddForce((targetPoint - player.transform.position).normalized * forceMultiplier, ForceMode2D.Impulse);
		prefabInstance = null;
	}
}
