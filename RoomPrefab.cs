using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomPrefab
{
	public GameObject prefab;

	public Vector2 size;

	public int minEnemiesPerWave = 1;

	public int maxEnemiesPerWave = 3;

	public List<Exit> exits = new List<Exit>();
}
