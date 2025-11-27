using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGlow : MonoBehaviour
{
	public GameObject spear;

	private List<AttackTelegraph> activeTelegraphs = new List<AttackTelegraph>();

	private void Start()
	{
	}

	private void Update()
	{
		activeTelegraphs.RemoveAll((AttackTelegraph item) => item == null);
	}

	private void OnEnable()
	{
		StartCoroutine(EnableGlow());
	}

	private void OnDisable()
	{
		CleanupTelegraphs();
	}

	private void CleanupTelegraphs()
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

	private IEnumerator EnableGlow()
	{
		SpriteRenderer sprite = GetComponent<SpriteRenderer>();
		Color color = sprite.color;
		while (color.a < 1f)
		{
			color.a += Time.deltaTime * 7f;
			sprite.color = color;
			yield return new WaitForSeconds(0.01f);
		}
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (gameObject != null)
		{
			Vector3 normalized = (gameObject.transform.position - base.transform.position).normalized;
			float angle = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			GameObject gameObject2 = new GameObject("AttackTelegraph");
			AttackTelegraph telegraph = gameObject2.AddComponent<AttackTelegraph>();
			telegraph.Initialize(base.transform.position, normalized, 20f, 1f, 0.1f);
			telegraph.preserveAfterDuration = true;
			activeTelegraphs.Add(telegraph);
			yield return new WaitForSeconds(0.4f);
			GameObject gameObject3 = Object.Instantiate(spear, base.transform.position, Quaternion.Euler(0f, 0f, angle));
			telegraph.AttachProjectile(gameObject3.transform);
		}
		else
		{
			yield return new WaitForSeconds(0.4f);
		}
		while (color.a > 0f)
		{
			color.a -= Time.deltaTime * 7f;
			sprite.color = color;
			yield return new WaitForSeconds(0.01f);
		}
		base.gameObject.SetActive(value: false);
	}
}
