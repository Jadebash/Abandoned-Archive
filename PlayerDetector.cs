using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
	public DungeonRoom room;

	private Dictionary<GameObject, Coroutine> exitChecks = new Dictionary<GameObject, Coroutine>();

	private void Start()
	{
		Refresh();
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		exitChecks.Clear();
	}

	public void Refresh()
	{
		if (room == null)
		{
			room = base.transform.parent.GetComponent<DungeonRoom>();
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (!(col.tag == "Player"))
		{
			return;
		}
		if (exitChecks.ContainsKey(col.gameObject))
		{
			if (exitChecks[col.gameObject] != null)
			{
				StopCoroutine(exitChecks[col.gameObject]);
			}
			exitChecks.Remove(col.gameObject);
		}
		Refresh();
		room.PlayerEntered(col.gameObject);
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if (!(col.tag == "Player"))
		{
			return;
		}
		if (exitChecks.ContainsKey(col.gameObject))
		{
			if (exitChecks[col.gameObject] != null)
			{
				StopCoroutine(exitChecks[col.gameObject]);
			}
			exitChecks.Remove(col.gameObject);
		}
		if (base.gameObject.activeInHierarchy && col.gameObject.activeInHierarchy)
		{
			exitChecks.Add(col.gameObject, StartCoroutine(CheckPlayerLeft(col.gameObject, GetComponent<Collider2D>())));
		}
	}

	private IEnumerator CheckPlayerLeft(GameObject player, Collider2D detectorCollider)
	{
		float tolerance = 1.6f;
		WaitForSeconds wait = new WaitForSeconds(0.1f);
		while (true)
		{
			if (player == null)
			{
				exitChecks.Remove(player);
				yield break;
			}
			Bounds bounds = detectorCollider.bounds;
			bounds.Expand(tolerance);
			if (!bounds.Contains(player.transform.position))
			{
				break;
			}
			yield return wait;
		}
		Refresh();
		if (room != null)
		{
			room.PlayerLeft();
		}
		exitChecks.Remove(player);
	}
}
