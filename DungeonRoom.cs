using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public abstract class DungeonRoom : MonoBehaviour
{
	public delegate void FinishedRoom();

	protected RoomInfo roomInfo;

	[HideInInspector]
	public bool spawnRelic;

	public Dictionary<Direction, GameObject> barriers = new Dictionary<Direction, GameObject>();

	[HideInInspector]
	public bool cleared;

	[HideInInspector]
	public bool active;

	public bool setupOnStart;

	public event FinishedRoom OnFinishedRoom;

	private void Awake()
	{
		roomInfo = GetComponent<RoomInfo>();
	}

	private void Start()
	{
		if (setupOnStart)
		{
			Setup();
		}
	}

	public void Setup()
	{
		try
		{
			barriers.Add(Direction.Up, roomInfo.upBarrier);
			barriers.Add(Direction.Right, roomInfo.rightBarrier);
			barriers.Add(Direction.Down, roomInfo.downBarrier);
			barriers.Add(Direction.Left, roomInfo.leftBarrier);
		}
		catch (Exception)
		{
			Debug.Log("Room barriers not found.");
			throw;
		}
		barriers[Direction.Up].SetActive(value: false);
		barriers[Direction.Down].SetActive(value: false);
		barriers[Direction.Left].SetActive(value: false);
		barriers[Direction.Right].SetActive(value: false);
		if (spawnRelic && roomInfo.relicSpawnPoints.Count > 0)
		{
			RunManager.Instance.SpawnRandomRelic(roomInfo.relicSpawnPoints[UnityEngine.Random.Range(0, roomInfo.relicSpawnPoints.Count)]);
		}
	}

	public void PlayerEntered(GameObject player)
	{
		Movement component = player.GetComponent<Movement>();
		if ((component != null && component.isTeleporting) || (FloorManager.Instance != null && FloorManager.Instance.isLoadingFloor))
		{
			return;
		}
		WaveManager.Instance?.WalkedRoom(base.transform);
		if (cleared || active)
		{
			return;
		}
		WaveManager.Instance?.EnteredRoom(this);
		if (!RoomStart())
		{
			return;
		}
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject != player)
			{
				Vector3 position = player.transform.position;
				Vector2 normalized = player.GetComponent<Rigidbody2D>().velocity.normalized;
				position += (Vector3)normalized * 0.5f;
				gameObject.GetComponent<Movement>().SafeTeleport(position);
			}
			gameObject.GetComponent<SpellCasting>().AwardKnowledgeForRoom();
		}
		if (!player.GetComponent<SpellCasting>().lowVisionRelic && !player.GetComponent<SpellCasting>().enemyHealRelic && !player.GetComponent<SpellCasting>().enemyBuffRelic && !player.GetComponent<SpellCasting>().spellFailRelic)
		{
			StartRoom();
			return;
		}
		if (player.GetComponent<SpellCasting>().visionCount < 5 && player.GetComponent<SpellCasting>().lowVisionRelic)
		{
			player.GetComponent<SpellCasting>().visionCount++;
			StartRoom();
		}
		else if (player.GetComponent<SpellCasting>().visionCount >= 5 && player.GetComponent<SpellCasting>().lowVisionRelic)
		{
			RelicCollector.Instance.DropRelic(player.GetComponent<SpellCasting>().visionRelic);
			player.GetComponent<SpellCasting>().visionCount = 0;
			StartRoom();
		}
		if (player.GetComponent<SpellCasting>().enemyHealRelic && player.GetComponent<SpellCasting>().enemyHealCount < 5)
		{
			player.GetComponent<SpellCasting>().enemyHealCount++;
			StartRoom();
		}
		else if (player.GetComponent<SpellCasting>().enemyHealRelic && player.GetComponent<SpellCasting>().enemyHealCount >= 5)
		{
			player.GetComponent<SpellCasting>().enemyHealCount = 0;
			RelicCollector.Instance.DropRelic(player.GetComponent<SpellCasting>().healEnemyRelic);
			StartRoom();
		}
		if (player.GetComponent<SpellCasting>().enemyBuffCount < 5 && player.GetComponent<SpellCasting>().enemyBuffRelic)
		{
			player.GetComponent<SpellCasting>().enemyBuffCount++;
			StartRoom();
		}
		else if (player.GetComponent<SpellCasting>().enemyBuffCount >= 5 && player.GetComponent<SpellCasting>().enemyBuffRelic)
		{
			player.GetComponent<SpellCasting>().enemyBuffCount = 0;
			RelicCollector.Instance.DropRelic(player.GetComponent<SpellCasting>().buffEnemyRelic);
			StartRoom();
		}
		if (player.GetComponent<SpellCasting>().spellFailCounter < 5 && player.GetComponent<SpellCasting>().spellFailRelic)
		{
			player.GetComponent<SpellCasting>().spellFailCounter++;
			StartRoom();
		}
		else if (player.GetComponent<SpellCasting>().spellFailCounter >= 5 && player.GetComponent<SpellCasting>().spellFailRelic)
		{
			player.GetComponent<SpellCasting>().spellFailCounter = 0;
			RelicCollector.Instance.DropRelic(player.GetComponent<SpellCasting>().failSpellRelic);
			StartRoom();
		}
	}

	public void PlayerLeft()
	{
		if (!cleared && !active)
		{
			PlayerLeftEarly();
		}
	}

	protected virtual void PlayerLeftEarly()
	{
	}

	public void StartRoom()
	{
		active = true;
		EnableBarriers();
	}

	protected virtual bool RoomStart()
	{
		return true;
	}

	protected void EnableBarriers()
	{
		LeanTween.value(base.gameObject, DissolveBarriers, 0.95f, 0f, 0.6f);
		foreach (Direction connection in roomInfo.connections)
		{
			barriers[connection].SetActive(value: true);
		}
	}

	protected void DisableBarriers()
	{
		LeanTween.value(base.gameObject, DissolveBarriers, 0f, 1f, 0.6f);
		RuntimeManager.CreateInstance("event:/SFX/World/BarrierBreak").start();
	}

	protected void RoomComplete()
	{
		this.OnFinishedRoom?.Invoke();
		if (FloorManager.Instance != null)
		{
			FloorManager.Instance.roomsClearedThisRun++;
		}
	}

	public void DissolveBarriers(float dissolve)
	{
		barriers[Direction.Up].GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", dissolve);
		barriers[Direction.Down].GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", dissolve);
		barriers[Direction.Left].GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", dissolve);
		barriers[Direction.Right].GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", dissolve);
		if (dissolve == 1f)
		{
			barriers[Direction.Up].SetActive(value: false);
			barriers[Direction.Down].SetActive(value: false);
			barriers[Direction.Left].SetActive(value: false);
			barriers[Direction.Right].SetActive(value: false);
		}
	}

	public void ResetRoom()
	{
		cleared = false;
		active = false;
		DisableBarriers();
		RoomReset();
	}

	protected virtual void RoomReset()
	{
	}
}
