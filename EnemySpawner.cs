using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public List<EnemySpawn> enemiesToSpawn = new List<EnemySpawn>();

	private float totalEnemySpawnRate;

	public float spawnRange = 1f;

	public float minSpawnRate = 10f;

	public float maxSpawnRate = 20f;

	private void Start()
	{
		foreach (EnemySpawn item in enemiesToSpawn)
		{
			totalEnemySpawnRate += item.spawnRateModifier;
		}
		InvokeRepeating("SpawnEnemy", Random.Range(0.5f, 3f), Random.Range(minSpawnRate, maxSpawnRate));
	}

	public void SpawnEnemy()
	{
		float num = Random.Range(0f, totalEnemySpawnRate);
		float num2 = 0f;
		foreach (EnemySpawn item in enemiesToSpawn)
		{
			num2 += item.spawnRateModifier;
			if (num <= num2)
			{
				Object.Instantiate(item.prefab, base.transform.position + (Vector3)Random.insideUnitCircle * spawnRange, Quaternion.identity);
				return;
			}
		}
		Debug.LogError($"No enemies to spawn in EnemySpawner. totalEnemySpawnRate: {totalEnemySpawnRate}, randomNum: {num}, total: {num2}, enemiesToSpawnCount: {enemiesToSpawn.Count}");
	}
}
