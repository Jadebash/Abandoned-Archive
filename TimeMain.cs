using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Time Main")]
public class TimeMain : Spell
{
	[HideInInspector]
	public float range = 7f;

	[HideInInspector]
	public float originalRange = 7f;

	[HideInInspector]
	public bool aoeBurst;

	[HideInInspector]
	public int bounces;

	[HideInInspector]
	public float movementSpeedInUseModifier;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			range *= 1.25f;
			originalRange = range;
			break;
		case 2:
			damage *= 1.4f;
			cooldownTime *= 1.25f;
			break;
		case 3:
			aoeBurst = true;
			break;
		case 4:
			bounces++;
			damage *= 0.9f;
			break;
		case 5:
			damage *= 1.4f;
			movementSpeedInUseModifier -= 0.3f * (1f + movementSpeedInUseModifier);
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		if ((bool)Object.FindObjectOfType<Rodh>())
		{
			range = originalRange * 1.5f;
		}
		GameObject gameObject = Object.Instantiate(prefab);
		gameObject.GetComponent<TetheringLine>().from = player.transform;
		gameObject.GetComponent<TetheringLine>().to = target.transform;
		gameObject.GetComponent<TetheringLine>().maxLength = range;
		gameObject.GetComponent<TetheringLine>().damagePerSecond = Mathf.RoundToInt(damage * damageMultiplier);
		gameObject.GetComponent<TetheringLine>().aoeDamage = Mathf.RoundToInt(damageMultiplier * gameObject.GetComponent<TetheringLine>().aoeDamage);
		gameObject.GetComponent<TetheringLine>().aoeDamageBurst = aoeBurst;
		gameObject.GetComponent<TetheringLine>().fromMovementSpeedModifier = movementSpeedInUseModifier;
		if (bounces > 0)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			List<GameObject> list = new List<GameObject>();
			GameObject[] array2 = array;
			foreach (GameObject item in array2)
			{
				list.Add(item);
			}
			list.Remove(target);
			GameObject gameObject2 = target;
			for (int j = 0; j < bounces; j++)
			{
				GameObject gameObject3 = null;
				float num = 7f;
				foreach (GameObject item2 in list)
				{
					float num2 = Vector3.Distance(item2.transform.position, gameObject2.transform.position);
					if (num2 < num)
					{
						gameObject3 = item2.gameObject;
						num = num2;
					}
				}
				if (gameObject3 != null)
				{
					list.Remove(gameObject3);
					GameObject gameObject4 = Object.Instantiate(prefab);
					gameObject4.GetComponent<TetheringLine>().from = gameObject2.transform;
					gameObject4.GetComponent<TetheringLine>().to = gameObject3.transform;
					gameObject4.GetComponent<TetheringLine>().maxLength = range;
					gameObject4.GetComponent<TetheringLine>().damagePerSecond = Mathf.RoundToInt(damage * damageMultiplier);
					gameObject4.GetComponent<TetheringLine>().aoeDamage = Mathf.RoundToInt(damageMultiplier * gameObject.GetComponent<TetheringLine>().aoeDamage);
					gameObject4.GetComponent<TetheringLine>().aoeDamageBurst = aoeBurst;
					gameObject2 = gameObject3;
				}
			}
		}
		CastAnimationOneHanded(player);
		Screenshake.Instance.AddTrauma(0.5f);
	}
}
