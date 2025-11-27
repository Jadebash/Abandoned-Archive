using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Lightning Main 2")]
public class LightningMain2 : Spell
{
	private Vector3 currentTarget;

	private GameObject prefabInstance;

	private List<GameObject> activeInstances = new List<GameObject>();

	private const int maxInstances = 3;

	[HideInInspector]
	public float forceMultiplier = 0.1f;

	[HideInInspector]
	public bool canShockPlayer;

	[HideInInspector]
	public float shockFreq = 0.5f;

	[HideInInspector]
	public float shockRange = 3f;

	[HideInInspector]
	public float shockDamageModifier = 2f;

	[HideInInspector]
	public float explosionDamageModifier = 1f;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			cooldownTime *= 0.75f;
			break;
		case 2:
			canShockPlayer = true;
			break;
		case 3:
			forceMultiplier *= 0.75f;
			shockFreq *= 1.2f;
			break;
		case 4:
			shockRange *= 1.3f;
			shockDamageModifier *= 1.25f;
			break;
		case 5:
			explosionDamageModifier *= 1.25f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		activeInstances.RemoveAll((GameObject item) => item == null);
		if (activeInstances.Count >= 3)
		{
			if (activeInstances[0] != null)
			{
				StickOnto component = activeInstances[0].GetComponent<StickOnto>();
				if (component != null)
				{
					component.Explode();
				}
				else
				{
					Object.Destroy(activeInstances[0]);
				}
			}
			activeInstances.RemoveAt(0);
		}
		currentTarget = targetPoint;
		prefabInstance = Object.Instantiate(prefab, player.transform.position, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, targetPoint - player.transform.position, Vector3.forward), Vector3.forward));
		activeInstances.Add(prefabInstance);
		prefabInstance.GetComponent<Rigidbody2D>().simulated = false;
		Screenshake.Instance.AddTrauma(1f);
		OrbOfThunder component2 = prefabInstance.GetComponent<OrbOfThunder>();
		component2.damageMultiplier = damageMultiplier;
		component2.canShockPlayer = canShockPlayer;
		component2.shockFreq = shockFreq;
		component2.shockRange = shockRange;
		component2.explosionDamageMoidifer = explosionDamageModifier;
		component2.shockDamageModifier = shockDamageModifier;
		component2.player = player;
		CastAnimationOneHanded(player);
		prefabInstance.GetComponent<Rigidbody2D>().simulated = true;
		prefabInstance.GetComponent<Rigidbody2D>().AddForce((currentTarget - player.transform.position).normalized * 25f * forceMultiplier, ForceMode2D.Impulse);
	}
}
