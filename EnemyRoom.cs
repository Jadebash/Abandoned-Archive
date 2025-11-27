using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : DungeonRoom
{
	public List<Dictionary<GameObject, int>> enemiesToSpawn = new List<Dictionary<GameObject, int>>();

	private List<GameObject> activeEnemies = new List<GameObject>();

	[HideInInspector]
	public bool spawnHealth;

	[HideInInspector]
	public bool spawnSpell;

	private int enemies;

	public bool bossRoom;

	protected override bool RoomStart()
	{
		if (enemiesToSpawn.Count <= 0 || enemiesToSpawn[0].Keys.Count <= 0)
		{
			return false;
		}
		MusicManager.EnterHighIntensity();
		SpawnEnemies();
		return true;
	}

	protected override void RoomReset()
	{
		enemies = 0;
		foreach (GameObject activeEnemy in activeEnemies)
		{
			if (activeEnemy != null)
			{
				Object.Destroy(activeEnemy);
			}
		}
	}

	private void SpawnEnemies()
	{
		enemies = 0;
		activeEnemies = new List<GameObject>();
		foreach (GameObject key in enemiesToSpawn[0].Keys)
		{
			for (int i = 0; i < enemiesToSpawn[0][key]; i++)
			{
				if (roomInfo.enemySpawnPoints.Count > 0)
				{
					Vector3 vector = roomInfo.enemySpawnPoints[Random.Range(0, roomInfo.enemySpawnPoints.Count)];
					GameObject gameObject = Object.Instantiate(key, vector + new Vector3(Random.insideUnitCircle.x * 0.4f, Random.insideUnitCircle.y * 0.4f), Quaternion.identity);
					gameObject.GetComponentInChildren<Health>().OnDeath += EnemyDeath;
					activeEnemies.Add(gameObject);
					enemies++;
				}
				else
				{
					Debug.LogWarning("No enemy spawn points");
				}
			}
		}
	}

	public void EnemyDeath(GameObject attacker)
	{
		enemies--;
		if (enemies > 0)
		{
			return;
		}
		enemies = 0;
		if (enemiesToSpawn.Count > 0)
		{
			enemiesToSpawn.RemoveAt(0);
		}
		if (enemiesToSpawn.Count > 0)
		{
			SpawnEnemies();
			return;
		}
		cleared = true;
		RoomComplete();
		SpawnRewards();
		WaveManager.Instance?.ExitedRoom(this);
		if (!bossRoom)
		{
			active = false;
			DisableBarriers();
		}
	}

	public void SpawnRewards()
	{
		Vector3 vector = ((!(roomInfo.eventRoomLever != null)) ? base.transform.position : roomInfo.eventRoomLever.transform.position);
		if (spawnHealth)
		{
			RunManager.Instance.SpawnHealthPickup(vector + Vector3.right * 0.6f);
		}
		if (spawnSpell)
		{
			if (roomInfo.landmarkRoom)
			{
				vector += new Vector3(2.5f, 0f, 0f);
			}
			RunManager.Instance.SpawnRandomSpell(vector);
		}
	}
}
