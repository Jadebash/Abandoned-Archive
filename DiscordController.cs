using Discord;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
	public static DiscordController Instance;

	public global::Discord.Discord discord;

	private bool connected;

	private void Awake()
	{
		Instance = this;
		try
		{
			discord = new global::Discord.Discord(714429942305390594L, 1uL);
			connected = true;
		}
		catch
		{
			Debug.Log("No discord found");
		}
	}

	private void Update()
	{
		if (connected && discord != null)
		{
			try
			{
				discord.RunCallbacks();
			}
			catch
			{
				Debug.LogWarning("Discord RunCallbacks error.");
			}
		}
	}

	public void UpdatePresence(string state, int runStartTimestamp)
	{
		try
		{
			if (!connected || discord == null)
			{
				return;
			}
			ActivityManager activityManager = discord.GetActivityManager();
			Activity activity = new Activity
			{
				State = state,
				Instance = true,
				Timestamps = 
				{
					Start = runStartTimestamp
				},
				Assets = 
				{
					LargeImage = "library_capsule",
					SmallImage = "logoaafinal"
				}
			};
			activityManager.UpdateActivity(activity, delegate(Result res)
			{
				if (res != Result.Ok)
				{
					Debug.LogError("Discord Activity Error");
				}
			});
		}
		catch
		{
			Debug.LogWarning("Could not update discord presence.");
		}
	}

	public void ClearPresence()
	{
		try
		{
			if (!connected || discord == null)
			{
				return;
			}
			discord.GetActivityManager().ClearActivity(delegate(Result res)
			{
				if (res != Result.Ok)
				{
					Debug.LogError("Discord Activity Clear Error");
				}
			});
		}
		catch
		{
			Debug.LogWarning("Could not clear discord presence.");
		}
	}

	private void OnDestroy()
	{
		try
		{
			if (discord != null)
			{
				discord.Dispose();
			}
		}
		catch
		{
			Debug.LogWarning("Could not dispose of discord.");
		}
	}
}
