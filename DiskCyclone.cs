using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public class DiskCyclone : MonoBehaviour
{
	private Vector3 castMousePos;

	public int bounces;

	public float projSpeed = 6f;

	public int bounceCount;

	public bool thrown;

	public GameObject explosionPrefab;

	public Vector3 target;

	public GameObject currentTarget;

	public GameObject previousTarget;

	public Vector3 previousTargetPosition;

	public float roomCollisionRadius = 0.25f;

	[HideInInspector]
	public GameObject player;

	private void FixedUpdate()
	{
		if ((bool)Physics2D.OverlapCircle(base.transform.position, roomCollisionRadius, LayerMask.GetMask("Room", "BossRoom")))
		{
			Explode();
		}
	}

	private void Update()
	{
		if (currentTarget != null && Vector3.Distance(base.transform.position, currentTarget.transform.position) < 8f)
		{
			previousTargetPosition = currentTarget.transform.position;
			base.transform.position += (currentTarget.transform.position - base.transform.position).normalized * Time.deltaTime * projSpeed;
			if (Vector3.Distance(base.transform.position, currentTarget.transform.position) <= 0.5f)
			{
				FinishedBounce();
			}
		}
		else
		{
			FinishedBounce();
		}
	}

	public void FinishedBounce()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Air_Main", base.transform.position);
		previousTarget = currentTarget;
		currentTarget = null;
		base.transform.localScale *= 0.9f;
		GetComponent<DamageOnEnter>().damage *= 0.9f;
		bounceCount++;
		if (bounceCount >= bounces)
		{
			Explode();
		}
		SetTarget(previousTargetPosition);
		if (currentTarget != null && currentTarget == previousTarget)
		{
			Explode();
		}
	}

	public void Explode()
	{
		if (explosionPrefab != null)
		{
			Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity);
		}
		Object.Destroy(base.gameObject);
	}

	public void SetTarget(Vector3 from)
	{
		List<GameObject> list = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		List<GameObject> list2 = new List<GameObject>();
		list.Remove(previousTarget);
		list = list.OrderBy((GameObject e) => Vector3.Distance(from, e.transform.position)).ToList();
		if (list.Count > 0)
		{
			foreach (GameObject item in list)
			{
				if (Vector3.Distance(from, item.transform.position) > 10f || item.GetComponent<Health>().invincible)
				{
					break;
				}
				list2.Add(item);
			}
			if (list2.Count == 0)
			{
				currentTarget = player;
				return;
			}
			if (list2.Count == 1)
			{
				list2.Add(player);
			}
			currentTarget = list2[0];
		}
		else
		{
			currentTarget = player;
		}
	}
}
