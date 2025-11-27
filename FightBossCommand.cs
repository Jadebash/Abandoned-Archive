using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fight Boss Command", menuName = "Abandoned Archive/Debug/Commands/Fight Boss Command")]
public class FightBossCommand : ConsoleCommand
{
	private class BossInfo
	{
		public int floor;

		public int bossIndex;

		public string displayName;

		public BossInfo(int floor, int bossIndex, string displayName)
		{
			this.floor = floor;
			this.bossIndex = bossIndex;
			this.displayName = displayName;
		}
	}

	private Dictionary<string, BossInfo> bosses = new Dictionary<string, BossInfo>
	{
		{
			"sirveltrine",
			new BossInfo(1, 0, "Sir Veltrine")
		},
		{
			"veltrine",
			new BossInfo(1, 0, "Sir Veltrine")
		},
		{
			"rufus",
			new BossInfo(1, 1, "Rufus")
		},
		{
			"lemures",
			new BossInfo(2, 0, "Lemures")
		},
		{
			"holite",
			new BossInfo(2, 1, "Holite")
		},
		{
			"crylia",
			new BossInfo(3, 0, "Crylia")
		},
		{
			"lostapparition",
			new BossInfo(3, 1, "Lost Apparition")
		},
		{
			"apparition",
			new BossInfo(3, 1, "Lost Apparition")
		},
		{
			"finalboss",
			new BossInfo(4, 0, "Final Boss")
		}
	};

	public override CommandResponse Process(string[] args)
	{
		if (args.Length == 0)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: fight_boss <boss_name> or fight_boss --list");
		}
		if (args[0].Equals("--list", StringComparison.OrdinalIgnoreCase))
		{
			return Success(GetBossList());
		}
		string key = args[0].ToLower();
		if (!bosses.ContainsKey(key))
		{
			return Fail(CommandFailType.Custom, "Boss '" + args[0] + "' not found. Use 'fight_boss --list' to see available bosses.");
		}
		BossInfo bossInfo = bosses[key];
		if (FloorManager.Instance == null)
		{
			return Fail(CommandFailType.Custom, "FloorManager not found. Make sure you're in the game.");
		}
		if (!FloorManager.Instance.playerSceneLoaded)
		{
			return Fail(CommandFailType.Custom, "Player scene not loaded. Start a run first before using this command.");
		}
		DeveloperConsoleBehaviour.Instance.StartCoroutine(LoadFloorAndFightBoss(bossInfo));
		return Success($"Loading floor {bossInfo.floor} to fight {bossInfo.displayName}...");
	}

	private string GetBossList()
	{
		string text = "<b>Available Bosses:</b>\n";
		text += "────────────────────────────────────────\n";
		Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (KeyValuePair<string, BossInfo> boss in bosses)
		{
			string key = boss.Key;
			BossInfo value = boss.Value;
			string item = value.floor + "_" + value.bossIndex;
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
				if (!dictionary.ContainsKey(value.floor))
				{
					dictionary[value.floor] = new List<string>();
				}
				dictionary[value.floor].Add("  <color=#00DDFF>" + value.displayName + "</color> - use: <color=#FFD700>" + key + "</color>");
			}
		}
		for (int i = 1; i <= 4; i++)
		{
			if (!dictionary.ContainsKey(i))
			{
				continue;
			}
			text += $"<b>Floor {i}:</b>\n";
			foreach (string item2 in dictionary[i])
			{
				text = text + item2 + "\n";
			}
			text += "\n";
		}
		return text + "────────────────────────────────────────\n";
	}

	private IEnumerator LoadFloorAndFightBoss(BossInfo bossInfo)
	{
		bool isFinalFloor = bossInfo.floor == 4;
		if (!isFinalFloor)
		{
			MapGenerator.SetForcedBossIndex(bossInfo.bossIndex);
		}
		WaveManager.preventAutoSpellGive = true;
		string scene;
		if (isFinalFloor)
		{
			scene = "Floor 4";
		}
		else
		{
			Floor floor = FloorManager.Instance.GetFloor(bossInfo.floor);
			if (floor == null)
			{
				Debug.LogError($"Floor {bossInfo.floor} not found in FloorManager.");
				yield break;
			}
			scene = floor.sceneName;
		}
		FloorManager.Instance.currentFloor = bossInfo.floor;
		FloorManager.Instance.StartLoadScene(scene, loadPlayer: false, !isFinalFloor, reloadPlayer: false, showFloorIntroText: false);
		bool loadingComplete = false;
		FloorManager.FinishedLoading onLoadComplete = delegate
		{
			loadingComplete = true;
		};
		FloorManager.Instance.OnFinishedLoading += onLoadComplete;
		while (!loadingComplete)
		{
			yield return null;
		}
		FloorManager.Instance.OnFinishedLoading -= onLoadComplete;
		WaveManager.preventAutoSpellGive = false;
		if (isFinalFloor)
		{
			Debug.Log("Loaded " + bossInfo.displayName + " floor. Floor 4 is a special scene - no boss room teleportation.");
			yield break;
		}
		yield return new WaitForEndOfFrame();
		float timeout = 10f;
		float elapsed = 0f;
		while ((MapGenerator.Instance == null || MapGenerator.Instance.bossRoomGenerationInformation == null) && elapsed < timeout)
		{
			yield return new WaitForSeconds(0.1f);
			elapsed += 0.1f;
		}
		if (MapGenerator.Instance == null || MapGenerator.Instance.bossRoomGenerationInformation == null)
		{
			Debug.LogError("Timeout waiting for MapGenerator to initialize boss room.");
		}
		else if (MapGenerator.Instance != null && MapGenerator.Instance.bossRoomGenerationInformation != null)
		{
			GameObject[] players = PlayerManager.players;
			foreach (GameObject gameObject in players)
			{
				if (gameObject != null)
				{
					Movement component = gameObject.GetComponent<Movement>();
					if (component != null)
					{
						component.SafeTeleport(MapGenerator.Instance.bossRoomGenerationInformation.exitPosition + new Vector3(0f, 3f, 0f));
					}
				}
			}
			RoomOptimiser[] array = UnityEngine.Object.FindObjectsOfType<RoomOptimiser>(includeInactive: true);
			for (int num = 0; num < array.Length; num++)
			{
				array[num].Optimise(null);
			}
			ShadowOptimiser[] array2 = UnityEngine.Object.FindObjectsOfType<ShadowOptimiser>();
			LightOptimiser[] array3 = UnityEngine.Object.FindObjectsOfType<LightOptimiser>();
			ShadowCasterOptimiser[] array4 = UnityEngine.Object.FindObjectsOfType<ShadowCasterOptimiser>();
			ShadowOptimiser[] array5 = array2;
			for (int num = 0; num < array5.Length; num++)
			{
				array5[num].UpdateLight();
			}
			LightOptimiser[] array6 = array3;
			for (int num = 0; num < array6.Length; num++)
			{
				array6[num].UpdateLight();
			}
			ShadowCasterOptimiser[] array7 = array4;
			for (int num = 0; num < array7.Length; num++)
			{
				array7[num].UpdateCaster();
			}
			Debug.Log($"Teleported to {bossInfo.displayName} boss room on floor {bossInfo.floor}.");
		}
		else
		{
			Debug.LogError("Failed to teleport to boss room - MapGenerator or boss room generation info not found.");
		}
	}
}
