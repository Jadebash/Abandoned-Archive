using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
	public delegate void KillEvent(GameObject killed);

	public List<string> tagToDamage = new List<string>();

	[HideInInspector]
	public List<Health> toDamage = new List<Health>();

	public float damagePerSecond;

	public float tickLength = 0.5f;

	public bool sendAttacker;

	public bool destroyProps = true;

	private float damageTimer;

	public event KillEvent OnKill;

	private void Update()
	{
		damageTimer -= Time.deltaTime;
		if (!(damageTimer <= 0f))
		{
			return;
		}
		damageTimer += tickLength;
		int num = Mathf.RoundToInt(damagePerSecond * tickLength);
		Health[] array = toDamage.ToArray();
		foreach (Health health in array)
		{
			if (health != null)
			{
				bool flag = false;
				if ((!sendAttacker) ? health.Damage(num) : health.Damage(num, base.gameObject))
				{
					this.OnKill?.Invoke(health.gameObject);
				}
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (destroyProps && col.tag == "PropBreakable" && (bool)col.GetComponent<PropBreak>())
		{
			col.GetComponent<PropBreak>().Break();
		}
		if (tagToDamage.Contains(col.transform.tag) && (bool)col.GetComponent<Health>())
		{
			toDamage.Add(col.GetComponent<Health>());
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if (tagToDamage.Contains(col.transform.tag) && (bool)col.GetComponent<Health>())
		{
			toDamage.Remove(col.GetComponent<Health>());
		}
	}
}
