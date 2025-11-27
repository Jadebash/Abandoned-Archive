using System.Collections.Generic;
using UnityEngine;

public static class CryliaTrapCoordinator
{
	private static readonly Dictionary<int, CryliaHoldSource> playerStates = new Dictionary<int, CryliaHoldSource>();

	private static readonly Dictionary<int, GameObject> activeTrapOwners = new Dictionary<int, GameObject>();

	public static bool PlayerHasState(GameObject player, CryliaHoldSource state)
	{
		return (GetState(player) & state) != 0;
	}

	public static void SetPlayerState(GameObject player, CryliaHoldSource source, bool active)
	{
		if (!(player == null))
		{
			int instanceID = player.GetInstanceID();
			CryliaHoldSource state = GetState(player);
			CryliaHoldSource cryliaHoldSource = (active ? (state | source) : (state & ~source));
			if (cryliaHoldSource == CryliaHoldSource.None)
			{
				playerStates.Remove(instanceID);
			}
			else
			{
				playerStates[instanceID] = cryliaHoldSource;
			}
		}
	}

	public static bool TryBeginTrap(GameObject player, GameObject trapOwner)
	{
		if (player == null || trapOwner == null)
		{
			return false;
		}
		if (IsPlayerOccupied(player))
		{
			return false;
		}
		int instanceID = player.GetInstanceID();
		if (activeTrapOwners.TryGetValue(instanceID, out var value) && value != trapOwner)
		{
			return false;
		}
		activeTrapOwners[instanceID] = trapOwner;
		SetPlayerState(player, CryliaHoldSource.Trap, active: true);
		return true;
	}

	public static void EndTrap(GameObject player, GameObject trapOwner)
	{
		ClearTrapOwner(player, trapOwner);
		SetPlayerState(player, CryliaHoldSource.Trap, active: false);
	}

	public static void ForceDestroyTrapForPlayer(GameObject player)
	{
		if (!PlayerHasState(player, CryliaHoldSource.Trap))
		{
			return;
		}
		GameObject trapOwner = GetTrapOwner(player);
		if (trapOwner == null)
		{
			SetPlayerState(player, CryliaHoldSource.Trap, active: false);
			return;
		}
		CryliaTrap component = trapOwner.GetComponent<CryliaTrap>();
		if (component != null)
		{
			component.ForceDestroyFromCoordinator();
		}
		else
		{
			EndTrap(player, trapOwner);
		}
	}

	public static bool IsPlayerOccupied(GameObject player)
	{
		return (GetState(player) & (CryliaHoldSource.Cage | CryliaHoldSource.Charge | CryliaHoldSource.Trap)) != 0;
	}

	private static CryliaHoldSource GetState(GameObject player)
	{
		if (player == null)
		{
			return CryliaHoldSource.None;
		}
		if (playerStates.TryGetValue(player.GetInstanceID(), out var value))
		{
			return value;
		}
		return CryliaHoldSource.None;
	}

	private static GameObject GetTrapOwner(GameObject player)
	{
		if (player == null)
		{
			return null;
		}
		int instanceID = player.GetInstanceID();
		if (activeTrapOwners.TryGetValue(instanceID, out var value))
		{
			return value;
		}
		return null;
	}

	private static void ClearTrapOwner(GameObject player, GameObject trapOwner)
	{
		if (!(player == null))
		{
			int instanceID = player.GetInstanceID();
			if (activeTrapOwners.TryGetValue(instanceID, out var value) && (trapOwner == null || value == trapOwner))
			{
				activeTrapOwners.Remove(instanceID);
			}
		}
	}
}
