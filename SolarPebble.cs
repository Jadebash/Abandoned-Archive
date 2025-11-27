using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolarPebble : MonoBehaviour
{
	public float lifeSpan;

	public float movementForce;

	public float AOERange;

	public float AOEDamage;

	public bool doDaze;

	public GameObject particles;

	private List<GameObject> attackList = new List<GameObject>();

	private bool killed;

	public bool canBounce;

	public float bounceDamageMod;

	public float bounceRangeMod;

	private void Start()
	{
		CoroutineEmitter.Instance.StartCoroutine(explodeAfterLifeSpan(lifeSpan));
		if (canBounce)
		{
			base.gameObject.GetComponent<CircleCollider2D>().sharedMaterial.bounciness = 1f;
		}
	}

	private IEnumerator explodeAfterLifeSpan(float lifeSpan)
	{
		yield return new WaitForSeconds(lifeSpan);
		base.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
		attackList = grabAttackList();
		foreach (GameObject attack in attackList)
		{
			if (!(attack == null))
			{
				if (doDaze)
				{
					attack.GetComponent<Enemy>()?.Stun(2f);
				}
				killed = attack.GetComponent<Health>().Damage(Mathf.RoundToInt(AOEDamage));
			}
		}
		Screenshake.Instance.AddTrauma(0.8f);
		Object.Instantiate(particles, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	private List<GameObject> grabAttackList()
	{
		List<GameObject> list = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		List<GameObject> list2 = new List<GameObject>();
		GameObject[] array = list.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(base.transform.position, gameObject.transform.position) < AOERange)
			{
				list2.Add(gameObject);
			}
		}
		return list2;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Room") && canBounce)
		{
			AOEDamage *= 1f + bounceDamageMod;
			AOERange *= 1f + bounceRangeMod;
		}
	}
}
