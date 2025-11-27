using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public delegate void GenerationComplete();

	public static MapGenerator Instance;

	[HideInInspector]
	public float generationProgress;

	private List<RoomGenerationInformation> bossRoomCandidates;

	[Header("Rooms")]
	[Space(10f)]
	[Header("Map Generation")]
	public RoomPrefab spawnRoom;

	public bool generateBossRoom;

	public BossRoomPrefab bossRoom;

	[HideInInspector]
	public GameObject bossRoomInstance;

	private static int forcedBossIndex = -1;

	private int predictedBossIndex = -1;

	public RoomPrefab connectorHorizontal;

	public RoomPrefab connectorVertical;

	public List<RoomPrefab> rooms;

	public List<RoomPrefab> floor1Landmarks;

	public List<RoomPrefab> floor2Landmarks;

	public List<RoomPrefab> floor3Landmarks;

	private List<RoomInfo> roomInstances;

	public LayerMask generationLayerMask;

	[Space(10f)]
	[Range(0f, 10f)]
	public int minRecursions = 2;

	[Range(0f, 10f)]
	public int maxRecursions = 3;

	[Range(0f, 100f)]
	public float chanceToDeadEnd;

	[Range(0f, 30f)]
	public int minRooms = 8;

	[Range(5f, 40f)]
	public int maxRooms = 12;

	[Header("Event Rooms")]
	[Range(0f, 10f)]
	public int numOfEventRooms = 10;

	public List<GameObject> eventRoomPrefabs;

	[Header("Spells")]
	[Range(0f, 20f)]
	public int numOfSpellsToSpawn = 10;

	[Header("Relics")]
	[Range(0f, 10f)]
	public int numberOfRelicsToSpawn = 5;

	[Header("Health Pickups")]
	[Range(0f, 20f)]
	public int healthPickupsToSpawn;

	[Header("Enemies")]
	public int maxNumberOfWaves = 1;

	public List<EnemyPrefab> enemyPrefabs = new List<EnemyPrefab>();

	private List<GameObject> prefabInstances;

	private bool landmarkRoomPlaced;

	private RoomInfo landmarkRoomInstance;

	[HideInInspector]
	public RoomGenerationInformation bossRoomGenerationInformation;

	[Header("Debug")]
	public bool optimiseAtEndOfFrame = true;

	public bool debugRoomNumbers;

	public bool dontOptimise;

	public event GenerationComplete OnGenerationComplete;

	private void Awake()
	{
		Instance = this;
		generationProgress = 0f;
	}

	public static void SetForcedBossIndex(int bossIndex)
	{
		forcedBossIndex = bossIndex;
	}

	private void Start()
	{
		if (!optimiseAtEndOfFrame || dontOptimise)
		{
			TipManager.ShowTip("Running in debug mode. Optimisation is disabled.", 5f);
		}
		generationProgress = 0f;
		bool flag = GenerateMap();
		int num = 1;
		while (!flag)
		{
			flag = RegenerateMap();
			num++;
			if (num >= 250)
			{
				generationProgress = 0f;
				Debug.LogError("Failed to generate map.");
				TipManager.ShowTip("Failed to generate map.");
				FloorManager.Instance.UnloadPlayer();
				FloorManager.Instance.StartLoadScene("Menu", loadPlayer: false, loadMap: false, reloadPlayer: false, showFloorIntroText: false);
				break;
			}
		}
		generationProgress = 100f;
		NodeGrid nodeGrid = UnityEngine.Object.FindObjectOfType<NodeGrid>();
		if (nodeGrid != null)
		{
			Bounds bounds = default(Bounds);
			foreach (RoomInfo roomInstance in roomInstances)
			{
				bounds.Encapsulate(roomInstance.transform.position);
			}
			nodeGrid.offset = bounds.center;
			nodeGrid.gridWorldSize.x = bounds.size.x + 20f;
			nodeGrid.gridWorldSize.y = bounds.size.y + 20f;
			nodeGrid.ResolveGrid();
		}
		else
		{
			Debug.LogWarning("No NodeGrid found, pathfinding will not work.");
		}
		if (optimiseAtEndOfFrame)
		{
			RoomOptimiser[] array = UnityEngine.Object.FindObjectsOfType<RoomOptimiser>(includeInactive: true);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OptimiseAtEndOfFrame(null);
			}
		}
		this.OnGenerationComplete?.Invoke();
	}

	private bool GenerateMap()
	{
		bossRoomCandidates = new List<RoomGenerationInformation>();
		roomInstances = new List<RoomInfo>();
		prefabInstances = new List<GameObject>();
		landmarkRoomPlaced = false;
		landmarkRoomInstance = null;
		predictedBossIndex = -1;
		GameObject item = UnityEngine.Object.Instantiate(spawnRoom.prefab, Vector3.zero, Quaternion.identity, base.transform);
		prefabInstances.Add(item);
		foreach (Exit exit in spawnRoom.exits)
		{
			GenerateRoom(exit.direction, exit.position, 0);
		}
		if (debugRoomNumbers)
		{
			Debug.Log(roomInstances.Count);
		}
		if (roomInstances.Count < minRooms || roomInstances.Count > maxRooms)
		{
			return false;
		}
		if (numOfEventRooms > 0)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			System.Random r = new System.Random();
			foreach (int item2 in from x in Enumerable.Range(0, roomInstances.Count)
				orderby r.Next()
				select x)
			{
				RoomInfo roomInfo = roomInstances[item2];
				if (roomInfo.landmarkRoom)
				{
					continue;
				}
				if ((bool)roomInfo.GetComponent<EnemyRoom>())
				{
					UnityEngine.Object.Destroy(roomInfo.GetComponent<EnemyRoom>());
					EventRoom eventRoom = roomInfo.gameObject.AddComponent<EventRoom>();
					GameObject gameObject = eventRoomPrefabs[UnityEngine.Random.Range(0, eventRoomPrefabs.Count)];
					if (gameObject == null)
					{
						Debug.LogError("Event room prefab is null");
						break;
					}
					while ((gameObject.name == "RoomOfAnarchy" && flag) || (gameObject.name == "RoomOfCartography" && flag2))
					{
						gameObject = eventRoomPrefabs[UnityEngine.Random.Range(0, eventRoomPrefabs.Count)];
						if (gameObject == null)
						{
							Debug.LogError("Event room prefab is null");
							break;
						}
					}
					if (gameObject.name == "RoomOfAnarchy")
					{
						flag = true;
					}
					if (gameObject.name == "RoomOfCartography")
					{
						flag2 = true;
					}
					eventRoom.eventRoomPrefab = gameObject;
					num++;
				}
				if (num >= numOfEventRooms)
				{
					break;
				}
			}
		}
		if (healthPickupsToSpawn > 0)
		{
			int num2 = 0;
			System.Random r2 = new System.Random();
			foreach (int item3 in from x in Enumerable.Range(0, roomInstances.Count)
				orderby r2.Next()
				select x)
			{
				RoomInfo roomInfo2 = roomInstances[item3];
				if ((bool)roomInfo2.GetComponent<EnemyRoom>())
				{
					roomInfo2.GetComponent<EnemyRoom>().spawnHealth = true;
					num2++;
				}
				if (num2 >= healthPickupsToSpawn)
				{
					break;
				}
			}
		}
		if (numOfSpellsToSpawn > 0)
		{
			int num3 = 0;
			System.Random r3 = new System.Random();
			foreach (int item4 in from x in Enumerable.Range(0, roomInstances.Count)
				orderby r3.Next()
				select x)
			{
				RoomInfo roomInfo3 = roomInstances[item4];
				if ((bool)roomInfo3.GetComponent<EnemyRoom>())
				{
					roomInfo3.GetComponent<EnemyRoom>().spawnSpell = true;
					num3++;
				}
				if (num3 >= numOfSpellsToSpawn)
				{
					break;
				}
			}
		}
		int availableRelicCount = RunManager.GetAvailableRelicCount();
		int num4 = Mathf.Min(numberOfRelicsToSpawn, availableRelicCount);
		if (num4 > 0)
		{
			int num5 = 0;
			System.Random r4 = new System.Random();
			foreach (int item5 in from x in Enumerable.Range(0, roomInstances.Count)
				orderby r4.Next()
				select x)
			{
				roomInstances[item5].GetComponent<DungeonRoom>().spawnRelic = true;
				num5++;
				if (num5 >= num4)
				{
					break;
				}
			}
		}
		if (generateBossRoom)
		{
			if (bossRoomCandidates.Count == 0)
			{
				Debug.LogWarning("No boss room candidates found.");
				return false;
			}
			bool flag3 = false;
			while (!flag3)
			{
				if (bossRoomCandidates.Count == 0)
				{
					Debug.LogWarning("All boss room candidates failed.");
					return false;
				}
				RoomGenerationInformation roomGenerationInformation = bossRoomCandidates[UnityEngine.Random.Range(0, bossRoomCandidates.Count)];
				bossRoomCandidates.Remove(roomGenerationInformation);
				flag3 = GenerateRoom(roomGenerationInformation.exitDirection, roomGenerationInformation.exitPosition, 0, roomGenerationInformation.previousRoom, doBossRoom: true);
				bossRoomGenerationInformation = roomGenerationInformation;
			}
		}
		foreach (RoomInfo roomInstance in roomInstances)
		{
			EventRoom component = roomInstance.GetComponent<EventRoom>();
			if (component != null)
			{
				component.Setup();
			}
			else
			{
				roomInstance.GetComponent<DungeonRoom>().Setup();
			}
		}
		return true;
	}

	private bool RegenerateMap()
	{
		Debug.LogWarning("Regenerating Map.");
		generationProgress = 0f;
		foreach (GameObject prefabInstance in prefabInstances)
		{
			UnityEngine.Object.Destroy(prefabInstance);
		}
		return GenerateMap();
	}

	private bool GenerateRoom(Direction exitDirection, Vector3 exitPosition, int recursion, RoomInfo previousRoom = null, bool doBossRoom = false)
	{
		generationProgress = Mathf.Clamp(generationProgress + 5f, 0f, 100f);
		if (recursion > maxRecursions)
		{
			if (generateBossRoom && exitDirection == Direction.Up)
			{
				bossRoomCandidates.Add(new RoomGenerationInformation(exitDirection, exitPosition, recursion, previousRoom));
			}
			return false;
		}
		if (recursion >= minRecursions && UnityEngine.Random.Range(0f, 100f) < chanceToDeadEnd && roomInstances.Count > minRooms)
		{
			return false;
		}
		if (roomInstances.Count >= maxRooms)
		{
			return false;
		}
		Direction direction = InverseDirection(exitDirection);
		RoomPrefab roomPrefab = null;
		switch (exitDirection)
		{
		case Direction.Up:
		case Direction.Down:
			roomPrefab = connectorVertical;
			break;
		case Direction.Right:
		case Direction.Left:
			roomPrefab = connectorHorizontal;
			break;
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		foreach (Exit exit in roomPrefab.exits)
		{
			if (exit.direction == direction)
			{
				vector = exit.position;
			}
			if (exit.direction == exitDirection)
			{
				vector2 = exit.position;
			}
		}
		RoomPrefab roomPrefab2;
		if (!doBossRoom)
		{
			if (!landmarkRoomPlaced && ShouldPlaceLandmarkRoom())
			{
				List<RoomPrefab> landmarkRoomsForCurrentFloor = GetLandmarkRoomsForCurrentFloor();
				List<RoomPrefab> list = FilterLandmarkRoomsWithMinExits(landmarkRoomsForCurrentFloor, 2);
				Debug.Log("Valid landmark rooms: " + list.Count);
				if (list.Count > 0)
				{
					roomPrefab2 = list[UnityEngine.Random.Range(0, list.Count)];
					if (FloorManager.Instance != null && FloorManager.Instance.currentFloor == 2)
					{
						roomPrefab2 = AdjustFloor2LandmarkForBoss(roomPrefab2, landmarkRoomsForCurrentFloor);
					}
				}
				else
				{
					roomPrefab2 = rooms[UnityEngine.Random.Range(0, rooms.Count)];
					landmarkRoomPlaced = true;
				}
			}
			else
			{
				roomPrefab2 = rooms[UnityEngine.Random.Range(0, rooms.Count)];
			}
		}
		else
		{
			roomPrefab2 = bossRoom;
		}
		Vector3 vector3 = Vector3.zero;
		foreach (Exit exit2 in roomPrefab2.exits)
		{
			if (exit2.direction == direction)
			{
				vector3 = exit2.position;
			}
		}
		Vector3 position = exitPosition - vector + vector2 - vector3;
		if (Physics2D.OverlapBoxAll(new Vector2(position.x, position.y), roomPrefab2.size, 0f, generationLayerMask.value).Length == 0)
		{
			GameObject item = UnityEngine.Object.Instantiate(roomPrefab.prefab, exitPosition - vector, Quaternion.identity, base.transform);
			GameObject gameObject = UnityEngine.Object.Instantiate(roomPrefab2.prefab, position, Quaternion.identity, base.transform);
			RoomInfo component = gameObject.GetComponent<RoomInfo>();
			EnemyRoom enemyRoom = component.gameObject.AddComponent<EnemyRoom>();
			component.recursion = recursion;
			roomInstances.Add(component);
			if (IsLandmarkRoom(roomPrefab2))
			{
				landmarkRoomInstance = component;
				landmarkRoomPlaced = true;
			}
			prefabInstances.Add(item);
			prefabInstances.Add(gameObject);
			if (doBossRoom)
			{
				enemyRoom.bossRoom = true;
				Dictionary<GameObject, int> dictionary = new Dictionary<GameObject, int>();
				int num;
				if (forcedBossIndex >= 0 && forcedBossIndex < ((BossRoomPrefab)roomPrefab2).bossPrefab.Length)
				{
					num = forcedBossIndex;
					forcedBossIndex = -1;
				}
				else if (predictedBossIndex >= 0 && predictedBossIndex < ((BossRoomPrefab)roomPrefab2).bossPrefab.Length)
				{
					num = predictedBossIndex;
					predictedBossIndex = -1;
				}
				else
				{
					num = UnityEngine.Random.Range(0, ((BossRoomPrefab)roomPrefab2).bossPrefab.Length);
				}
				dictionary.Add(((BossRoomPrefab)roomPrefab2).bossPrefab[num], 1);
				enemyRoom.enemiesToSpawn.Add(dictionary);
				bossRoomInstance = gameObject;
			}
			else if (roomPrefab2.maxEnemiesPerWave > 0)
			{
				int num2 = UnityEngine.Random.Range(1, maxNumberOfWaves + 1);
				for (int i = 1; i <= num2; i++)
				{
					Dictionary<GameObject, int> dictionary2 = new Dictionary<GameObject, int>();
					int num3 = UnityEngine.Random.Range(roomPrefab2.minEnemiesPerWave, roomPrefab2.maxEnemiesPerWave + 1) * FloorManager.Instance.currentLoop;
					int num4 = 0;
					int num5 = 0;
					while (num4 < num3)
					{
						EnemyPrefab enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
						int num6 = 0;
						if (dictionary2.ContainsKey(enemyPrefab.prefab))
						{
							num6 = dictionary2[enemyPrefab.prefab];
						}
						if (enemyPrefab.max * FloorManager.Instance.currentLoop - num6 == 0)
						{
							num5++;
							if (num5 > 10)
							{
								Debug.LogWarning("Could not complete enemy spawning of " + num3 + " enemies.");
								break;
							}
						}
						else
						{
							int num7 = UnityEngine.Random.Range(1, Mathf.Min(num3 - num4, enemyPrefab.max * FloorManager.Instance.currentLoop - num6) + 1);
							num4 += num7;
							if (num6 > 0)
							{
								dictionary2[enemyPrefab.prefab] += num7;
							}
							else
							{
								dictionary2.Add(enemyPrefab.prefab, num7);
							}
						}
					}
					enemyRoom.enemiesToSpawn.Add(dictionary2);
				}
			}
			int num8 = 0;
			foreach (Exit item2 in roomPrefab2.exits.OrderBy((Exit x) => UnityEngine.Random.Range(0f, 100f)))
			{
				if (item2.direction == direction)
				{
					DisableRoomBlock(item2.direction, component);
					if (doBossRoom)
					{
						DisableRoomBlock(exitDirection, previousRoom);
					}
					num8++;
				}
				else if (!doBossRoom && GenerateRoom(item2.direction, gameObject.transform.position + item2.position, recursion + 1, component))
				{
					DisableRoomBlock(item2.direction, component);
					num8++;
				}
			}
			if (landmarkRoomInstance == component && num8 < 2)
			{
				Debug.LogWarning($"Landmark room only has {num8} connections, expected at least 2. This may affect gameplay.");
			}
			return true;
		}
		return false;
	}

	private void DisableRoomBlock(Direction direction, RoomInfo room)
	{
		room.connections.Add(direction);
		switch (direction)
		{
		case Direction.Up:
			room.upBlock.SetActive(value: false);
			break;
		case Direction.Right:
			room.rightBlock.SetActive(value: false);
			break;
		case Direction.Down:
			room.downBlock.SetActive(value: false);
			break;
		case Direction.Left:
			room.leftBlock.SetActive(value: false);
			break;
		}
	}

	private Direction InverseDirection(Direction directionToInverse)
	{
		switch (directionToInverse)
		{
		case Direction.Up:
			return Direction.Down;
		case Direction.Right:
			return Direction.Left;
		case Direction.Down:
			return Direction.Up;
		case Direction.Left:
			return Direction.Right;
		default:
			Debug.LogError("Inverse Direction Error");
			return Direction.Up;
		}
	}

	private bool ShouldPlaceLandmarkRoom()
	{
		if (roomInstances.Count >= 2)
		{
			return roomInstances.Count <= 6;
		}
		return false;
	}

	private List<RoomPrefab> GetLandmarkRoomsForCurrentFloor()
	{
		if (FloorManager.Instance == null)
		{
			Debug.LogWarning("FloorManager instance not found, using floor 1 landmarks");
			return floor1Landmarks;
		}
		return FloorManager.Instance.currentFloor switch
		{
			1 => floor1Landmarks, 
			2 => floor2Landmarks, 
			3 => floor3Landmarks, 
			_ => floor1Landmarks, 
		};
	}

	private List<RoomPrefab> FilterLandmarkRoomsWithMinExits(List<RoomPrefab> landmarkRooms, int minExits)
	{
		List<RoomPrefab> list = new List<RoomPrefab>();
		if (landmarkRooms == null)
		{
			return list;
		}
		foreach (RoomPrefab landmarkRoom in landmarkRooms)
		{
			if (landmarkRoom != null && landmarkRoom.exits != null && landmarkRoom.exits.Count >= minExits)
			{
				list.Add(landmarkRoom);
			}
		}
		return list;
	}

	private bool IsLandmarkRoom(RoomPrefab room)
	{
		return GetLandmarkRoomsForCurrentFloor().Contains(room);
	}

	private RoomPrefab AdjustFloor2LandmarkForBoss(RoomPrefab selectedRoom, List<RoomPrefab> landmarkRooms)
	{
		if (floor2Landmarks.Count > 2 && selectedRoom != floor2Landmarks[2])
		{
			int num;
			if (forcedBossIndex >= 0 && bossRoom != null && forcedBossIndex < bossRoom.bossPrefab.Length)
			{
				num = forcedBossIndex;
			}
			else
			{
				if (predictedBossIndex < 0 && bossRoom != null && bossRoom.bossPrefab.Length != 0)
				{
					predictedBossIndex = UnityEngine.Random.Range(0, bossRoom.bossPrefab.Length);
				}
				num = predictedBossIndex;
			}
			if (num >= 0 && num < bossRoom.bossPrefab.Length && bossRoom.bossPrefab[num] != null)
			{
				string text = bossRoom.bossPrefab[num].name.ToLower();
				if ((text.Contains("lemuresfight") || text.Contains("lemures")) && floor2Landmarks.Count > 0)
				{
					if (floor2Landmarks[0] != null && floor2Landmarks[0].exits != null && floor2Landmarks[0].exits.Count >= 2)
					{
						return floor2Landmarks[0];
					}
				}
				else if (text.Contains("holite") && floor2Landmarks.Count > 1 && floor2Landmarks[1] != null && floor2Landmarks[1].exits != null && floor2Landmarks[1].exits.Count >= 2)
				{
					return floor2Landmarks[1];
				}
			}
		}
		return selectedRoom;
	}
}
