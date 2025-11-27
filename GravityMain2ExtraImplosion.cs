using System;
using System.Collections.Generic;
using UnityEngine;

public class GravityMain2ExtraImplosion : MonoBehaviour
{
	public float AOEDamageSize;

	public float force;

	public float AOEDamage;

	public float stunChance;

	public GameObject extraImplosionPrefab;

	public GameObject particles;

	public List<GameObject> attackList = new List<GameObject>();

	private void Start()
	{
		GameObject[] array = attackList.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				continue;
			}
			gameObject.GetComponent<Enemy>()?.Stun(0.3f);
			gameObject.GetComponent<Rigidbody2D>()?.AddForce((base.transform.position - gameObject.transform.position) * force, ForceMode2D.Impulse);
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) < 2f)
			{
				gameObject.GetComponent<Health>()?.Damage(Mathf.RoundToInt(AOEDamage));
				if ((float)new System.Random().Next(1, 100) < stunChance)
				{
					gameObject.GetComponent<Enemy>()?.Stun(2f);
				}
			}
		}
		Screenshake.Instance.AddTrauma(0.8f);
		UnityEngine.Object.Instantiate(particles, base.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
