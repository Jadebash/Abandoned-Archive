using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DungeonRoom))]
public class AddEnemiesToSpawn : MonoBehaviour
{
	public List<AddWaveToSpawn> enemiesToSpawn = new List<AddWaveToSpawn>();

	private void Awake()
	{
		List<Dictionary<GameObject, int>> list = new List<Dictionary<GameObject, int>>();
		foreach (AddWaveToSpawn item in enemiesToSpawn)
		{
			Dictionary<GameObject, int> dictionary = new Dictionary<GameObject, int>();
			foreach (AddEnemyToSpawn enemy in item.enemies)
			{
				dictionary.Add(enemy.prefab, enemy.num);
			}
			list.Add(dictionary);
		}
		GetComponent<EnemyRoom>().enemiesToSpawn = list;
	}
}
