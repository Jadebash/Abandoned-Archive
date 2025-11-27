using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Dev Spell")]
public class DevSpell : Spell
{
	public int bounces = 3;

	public float bounceDistance = 4f;

	[Tooltip("Damage falloff formula is damagePerSecond = previousDamagePerSecond / ((bounces * damageFalloff) + 2)")]
	public float damageFalloff = 1f;

	public int stunEnemyCount;

	public float range = 4.5f;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			bounces++;
			break;
		case 2:
			damageFalloff /= 2f;
			bounceDistance *= 0.75f;
			break;
		case 3:
			damage *= 1.5f;
			damageFalloff *= 1.25f;
			break;
		case 4:
			stunEnemyCount++;
			cooldownTime *= 1.5f;
			break;
		case 5:
			damage *= 1.25f;
			damageFalloff *= 0.5f;
			bounces--;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		Vector2 vector = target.transform.position - player.transform.position;
		if ((bool)Object.FindObjectOfType<Rodh>())
		{
			range = 8f;
		}
		if (!(vector.magnitude <= range))
		{
			return;
		}
		Screenshake.Instance.AddTrauma(0.6f);
		GameObject gameObject = Object.Instantiate(prefab);
		gameObject.GetComponent<TetheringLine>().from = player.transform;
		gameObject.GetComponent<TetheringLine>().to = target.transform;
		int num = (int)Mathf.Round(damage * damageMultiplier);
		gameObject.GetComponent<TetheringLine>().damagePerSecond = num;
		if (stunEnemyCount > 0 && (bool)target.GetComponent<Enemy>())
		{
			target.GetComponent<Enemy>().Stun(0.5f);
		}
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Lightning_Main", gameObject.transform.position);
		CastAnimationOneHanded(player);
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
			float num2 = bounceDistance;
			foreach (GameObject item2 in list)
			{
				float num3 = Vector3.Distance(item2.transform.position, gameObject2.transform.position);
				if (num3 < num2)
				{
					gameObject3 = item2.gameObject;
					num2 = num3;
				}
			}
			if (gameObject3 != null)
			{
				list.Remove(gameObject3);
				GameObject gameObject4 = Object.Instantiate(prefab);
				gameObject4.GetComponent<TetheringLine>().from = gameObject2.transform;
				gameObject4.GetComponent<TetheringLine>().to = gameObject3.transform;
				num = (int)Mathf.Round(Mathf.Clamp((float)num / ((float)j * damageFalloff + 2f), 1f, float.PositiveInfinity) * damageMultiplier);
				gameObject4.GetComponent<TetheringLine>().damagePerSecond = num;
				if ((bool)gameObject3.GetComponent<Enemy>() && j + 1 < stunEnemyCount)
				{
					gameObject3.GetComponent<Enemy>().Stun(0.5f);
				}
				gameObject2 = gameObject3;
			}
		}
	}
}
