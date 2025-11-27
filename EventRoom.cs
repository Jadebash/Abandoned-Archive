using FMODUnity;
using UnityEngine;

public class EventRoom : DungeonRoom
{
	public GameObject eventRoomPrefab;

	private GameObject eventRoomInstance;

	protected override bool RoomStart()
	{
		if (roomInfo.eventRoomLever != null)
		{
			roomInfo.eventRoomLever.SetActive(value: true);
		}
		return false;
	}

	private void TeleportOtherPlayers()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (gameObject == null)
		{
			return;
		}
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject2 in players)
		{
			if (gameObject2 != gameObject)
			{
				Vector3 position = gameObject.transform.position;
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				if (component != null)
				{
					Vector2 normalized = component.velocity.normalized;
					position += (Vector3)normalized * 0.5f;
				}
				Movement component2 = gameObject2.GetComponent<Movement>();
				if (component2 != null)
				{
					component2.SafeTeleport(position);
				}
			}
		}
	}

	protected override void PlayerLeftEarly()
	{
		if (roomInfo.eventRoomLever != null)
		{
			roomInfo.eventRoomLever.GetComponent<Animator>().SetTrigger("Destroy");
			WaveManager.Instance?.ExitedRoom(this);
			cleared = true;
			RoomComplete();
		}
	}

	public void DisablePillars()
	{
		GameObject[] array = roomInfo.pillars.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public void EnablePillars()
	{
		GameObject[] array = roomInfo.pillars.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	public void StartEvent()
	{
		DisablePillars();
		TeleportOtherPlayers();
		StartRoom();
		if (eventRoomPrefab != null)
		{
			eventRoomInstance = Object.Instantiate(eventRoomPrefab, base.transform.TransformPoint(base.gameObject.GetComponent<RoomInfo>().eventRoomPosition), Quaternion.identity, base.transform);
			RuntimeManager.StudioSystem.setParameterByName("InEvent", 1f);
		}
		else
		{
			Debug.LogError("Event room prefab is null");
			EndEvent();
		}
	}

	public void EndEvent()
	{
		WaveManager.Instance?.ExitedRoom(this);
		EnablePillars();
		DisableBarriers();
		cleared = true;
		RoomComplete();
		RuntimeManager.StudioSystem.setParameterByName("InEvent", 0f);
		if (eventRoomInstance.transform.childCount > 0)
		{
			eventRoomInstance.transform.GetChild(0).GetComponent<Animator>().SetTrigger("End");
		}
	}
}
