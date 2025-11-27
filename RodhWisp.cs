using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodhWisp : MonoBehaviour
{
	public GameObject spear;

	private float shootTimer;

	public Transform spawnPos;

	private float deathTimer = 10f;

	private List<AttackTelegraph> activeTelegraphs = new List<AttackTelegraph>();

	private void Start()
	{
		shootTimer = Random.Range(0.4f, 2.2f);
	}

	private void Update()
	{
		shootTimer -= Time.deltaTime;
		deathTimer -= Time.deltaTime;
		if (shootTimer <= 0f)
		{
			shootTimer = Random.Range(0.4f, 2.2f) + 0.3f;
			StartCoroutine(AttackRoutine());
		}
		if (deathTimer <= 0f)
		{
			deathTimer = 20f;
			GetComponent<Animator>().SetTrigger("Death");
		}
		activeTelegraphs.RemoveAll((AttackTelegraph item) => item == null);
	}

	private IEnumerator AttackRoutine()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (gameObject != null)
		{
			Vector3 normalized = (gameObject.transform.position - base.transform.position).normalized;
			float angle = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			GameObject gameObject2 = new GameObject("AttackTelegraph");
			AttackTelegraph telegraph = gameObject2.AddComponent<AttackTelegraph>();
			Vector3 origin = ((spawnPos != null) ? spawnPos.position : base.transform.position);
			telegraph.Initialize(origin, normalized, 20f, 0.4f, 0.1f);
			telegraph.preserveAfterDuration = true;
			activeTelegraphs.Add(telegraph);
			yield return new WaitForSeconds(0.4f);
			GameObject gameObject3 = Object.Instantiate(spear, origin, Quaternion.Euler(0f, 0f, angle));
			telegraph.AttachProjectile(gameObject3.transform);
		}
	}

	public void Death()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		foreach (AttackTelegraph activeTelegraph in activeTelegraphs)
		{
			if (activeTelegraph != null)
			{
				Object.Destroy(activeTelegraph.gameObject);
			}
		}
		activeTelegraphs.Clear();
	}
}
