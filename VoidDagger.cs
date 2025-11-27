using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public class VoidDagger : MonoBehaviour
{
	[HideInInspector]
	public GameObject player;

	[HideInInspector]
	public float moveSpeed = 20f;

	private GameObject targetEnemy;

	public Vector3 from;

	private bool destroyed;

	public float damage;

	public float stunTime = 0.1f;

	public void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		targetEnemy = findClosestEnemy();
		if (targetEnemy == null)
		{
			atEnemy();
		}
	}

	private void Update()
	{
		if (base.gameObject == null)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Gravity_Main", player.transform.position);
		}
		if (!destroyed && targetEnemy != null)
		{
			if (Vector3.Distance(base.transform.position, targetEnemy.transform.position) < 0.4f)
			{
				targetEnemy.GetComponent<Enemy>().Stun(stunTime);
				targetEnemy.GetComponent<Health>().Damage(damage);
				atEnemy();
			}
			else if (targetEnemy != null)
			{
				base.transform.position += (from - base.transform.position).normalized * Time.deltaTime * moveSpeed;
			}
		}
		else
		{
			atEnemy();
		}
	}

	public GameObject findClosestEnemy()
	{
		List<GameObject> source = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		if (source.Count() > 0)
		{
			source = source.OrderBy((GameObject e) => Vector3.Distance(from, e.transform.position)).ToList();
			Debug.Log(source[0].GetComponent<Enemy>());
			return source[0];
		}
		return null;
	}

	public void atEnemy()
	{
		destroyed = true;
		Object.Destroy(base.gameObject);
	}
}
