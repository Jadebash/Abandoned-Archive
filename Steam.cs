using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class Steam : MonoBehaviour
{
	public delegate void Init();

	public static Steam Instance;

	[HideInInspector]
	public bool verified;

	public event Init OnInit;

	private void Awake()
	{
		Instance = this;
		try
		{
			uint appid = 1817500u;
			if (Manager.Instance != null && Manager.Instance.isDemo)
			{
				appid = 1879090u;
			}
			SteamClient.Init(appid);
			this.OnInit?.Invoke();
			verified = true;
		}
		catch
		{
			Debug.LogError("Steamworks failed to init.");
			Application.Quit();
		}
	}

	public static void TriggerAchievement(string achievementId)
	{
		if (Instance == null || !Instance.verified || DeveloperConsole.hasBeenUsedThisSession)
		{
			return;
		}
		foreach (Achievement achievement in SteamUserStats.Achievements)
		{
			if (achievement.Identifier == achievementId)
			{
				if (!achievement.State)
				{
					achievement.Trigger();
				}
				return;
			}
		}
		Debug.LogWarning("Couldn't find achievement with ID " + achievementId);
	}

	private void Update()
	{
		SteamClient.RunCallbacks();
	}

	private void OnApplicationQuit()
	{
		SteamClient.Shutdown();
	}

	public bool IsSteamDeck()
	{
		return SteamUtils.IsRunningOnSteamDeck;
	}
}
