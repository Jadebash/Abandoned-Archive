using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
	public List<Direction> connections = new List<Direction>();

	[HideInInspector]
	public List<Vector2> relicSpawnPoints = new List<Vector2>();

	[HideInInspector]
	public List<Vector2> enemySpawnPoints = new List<Vector2>();

	public bool landmarkRoom;

	public GameObject upBlock;

	public GameObject downBlock;

	public GameObject leftBlock;

	public GameObject rightBlock;

	public GameObject upBarrier;

	public GameObject downBarrier;

	public GameObject leftBarrier;

	public GameObject rightBarrier;

	[HideInInspector]
	public int recursion;

	[Header("Event Rooms")]
	public GameObject eventRoomLever;

	public Vector3 eventRoomPosition = new Vector3(0f, 0f, 0f);

	public List<GameObject> pillars;

	private void Awake()
	{
		Transform transform = base.transform.Find("RelicSpawnPoints");
		relicSpawnPoints = new List<Vector2>();
		if (transform != null)
		{
			foreach (Transform item in transform)
			{
				if (item.gameObject.activeSelf)
				{
					relicSpawnPoints.Add(item.position);
				}
			}
		}
		Transform transform3 = base.transform.Find("EnemySpawnPoints");
		enemySpawnPoints = new List<Vector2>();
		if (!(transform3 != null))
		{
			return;
		}
		foreach (Transform item2 in transform3)
		{
			if (item2.gameObject.activeSelf)
			{
				enemySpawnPoints.Add(item2.position);
			}
		}
	}
}
