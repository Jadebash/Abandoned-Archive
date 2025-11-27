using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
	[HideInInspector]
	public List<Enemy> enemiesInDetector;

	public void OnTriggerEnter2D(Collider2D col)
	{
		if ((bool)col.GetComponent<Enemy>())
		{
			enemiesInDetector.Add(col.GetComponent<Enemy>());
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if ((bool)col.GetComponent<Enemy>())
		{
			enemiesInDetector.Remove(col.GetComponent<Enemy>());
		}
	}
}
