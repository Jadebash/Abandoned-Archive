using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public class PressurePiston : MonoBehaviour
{
	private Vector3 currentTarget;

	private bool atTarget;

	private bool hasExploded;

	private float projSpeed = 5f;

	private float force = 24f;

	public float AOEDamageSize = 4f;

	private float AOEDamage = 4f;

	public float stunChance;

	public bool doExtraImplosion;

	[HideInInspector]
	public bool activated;

	public GameObject particles;

	public GameObject extraImplosionPrefab;

	public bool moreDamagePerEnemy;

	public int extraImplosionsToDo;

	public float roomCollisionRadius = 0.25f;

	private void FixedUpdate()
	{
		if (activated && !hasExploded && (bool)Physics2D.OverlapCircle(base.transform.position, roomCollisionRadius, LayerMask.GetMask("Room", "BossRoom")))
		{
			Explode();
		}
	}

	public void Explode()
	{
		if (!activated || hasExploded)
		{
			return;
		}
		hasExploded = true;
		List<GameObject> list = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		List<GameObject> list2 = new List<GameObject>();
		GameObject[] array = list.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(base.transform.position, gameObject.transform.position) < AOEDamageSize)
			{
				list2.Add(gameObject);
			}
		}
		if (moreDamagePerEnemy)
		{
			AOEDamage *= 1 + list2.Count() / 2;
		}
		array = list2.ToArray();
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 == null)
			{
				continue;
			}
			try
			{
				if (!(Vector3.Distance(gameObject2.transform.position, base.transform.position) > 0.01f))
				{
					continue;
				}
				bool flag = false;
				gameObject2.GetComponent<Enemy>()?.Stun(0.3f);
				Rigidbody2D component = gameObject2.GetComponent<Rigidbody2D>();
				if (component != null && Time.timeScale != 0f)
				{
					component.AddForce((base.transform.position - gameObject2.transform.position) * force, ForceMode2D.Impulse);
					component.velocity = Vector2.ClampMagnitude(component.velocity, 15f);
				}
				if (Vector3.Distance(gameObject2.transform.position, base.transform.position) < 2f)
				{
					flag = gameObject2.GetComponent<Health>()?.Damage(Mathf.RoundToInt(AOEDamage)) ?? false;
					if ((float)new System.Random().Next(1, 100) < stunChance)
					{
						gameObject2.GetComponent<Enemy>()?.Stun(2f);
					}
				}
				if (flag && doExtraImplosion)
				{
					CoroutineEmitter.Instance.StartCoroutine(spawnImplosion(list2, gameObject2.transform.position));
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[PressurePiston]: " + ex.Message);
			}
		}
		if (extraImplosionsToDo > 0)
		{
			CoroutineEmitter.Instance.StartCoroutine(spawnExtraImplosions(list2, base.transform.position));
		}
		Screenshake.Instance.AddTrauma(0.8f);
		UnityEngine.Object.Instantiate(particles, base.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!activated || hasExploded)
		{
			return;
		}
		_ = currentTarget;
		if (!atTarget)
		{
			base.transform.position += (currentTarget - base.transform.position).normalized * Time.deltaTime * projSpeed;
			if (Vector3.Distance(base.transform.position, currentTarget) < 0.1f)
			{
				atTarget = true;
				Explode();
			}
		}
	}

	public void setTarget(Vector3 target, float chargeTimer, float damageModifier)
	{
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Water_Main_Throw", base.transform.position);
		chargeTimer = 0.5f + chargeTimer / 2f;
		AOEDamage = Mathf.RoundToInt(AOEDamage * damageModifier * chargeTimer);
		force *= chargeTimer;
		activated = true;
		currentTarget = target;
	}

	private IEnumerator spawnImplosion(List<GameObject> attackList, Vector3 location)
	{
		yield return new WaitForSeconds(0.25f);
		GameObject obj = UnityEngine.Object.Instantiate(extraImplosionPrefab, location, Quaternion.identity);
		obj.GetComponent<GravityMain2ExtraImplosion>().force = force / 10f;
		obj.GetComponent<GravityMain2ExtraImplosion>().AOEDamage = AOEDamage / 10f;
		obj.GetComponent<GravityMain2ExtraImplosion>().AOEDamageSize = AOEDamageSize;
		obj.GetComponent<GravityMain2ExtraImplosion>().stunChance = stunChance / 10f;
		obj.GetComponent<GravityMain2ExtraImplosion>().attackList = attackList;
		obj.transform.parent = null;
	}

	private IEnumerator spawnExtraImplosions(List<GameObject> attackList, Vector3 location)
	{
		for (int i = 0; i < extraImplosionsToDo; i++)
		{
			yield return new WaitForSeconds(0.25f);
			GameObject obj = UnityEngine.Object.Instantiate(extraImplosionPrefab, location, Quaternion.identity);
			obj.GetComponent<GravityMain2ExtraImplosion>().force = force / 10f;
			obj.GetComponent<GravityMain2ExtraImplosion>().AOEDamage = AOEDamage / 10f;
			obj.GetComponent<GravityMain2ExtraImplosion>().AOEDamageSize = AOEDamageSize;
			obj.GetComponent<GravityMain2ExtraImplosion>().stunChance = stunChance / 10f;
			obj.GetComponent<GravityMain2ExtraImplosion>().attackList = attackList;
			obj.transform.parent = null;
		}
	}
}
