using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
	public delegate void Pause(bool paused);

	public static Manager Instance;

	private FloorManager floorManager;

	[HideInInspector]
	public bool isPaused;

	private float lastTimescale = 1f;

	[HideInInspector]
	public string LastKnownControlScheme = "Keyboard&Mouse";

	public bool isDemo;

	public event Pause OnPause;

	private void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		floorManager = GetComponent<FloorManager>();
		if (!(SceneManager.GetActiveScene().name == "Manager"))
		{
			return;
		}
		if (Steam.Instance != null)
		{
			if (Steam.Instance.verified)
			{
				Verified();
			}
			else
			{
				Steam.Instance.OnInit += Verified;
			}
			return;
		}
		BetaRequirement component = GetComponent<BetaRequirement>();
		if ((bool)component)
		{
			component.OnVerify += Verified;
		}
		else
		{
			floorManager.StartLoadScene("Menu", loadPlayer: false, loadMap: false, reloadPlayer: false, showFloorIntroText: false);
		}
	}

	private void Verified()
	{
		floorManager.StartLoadScene("Menu", loadPlayer: false, loadMap: false, reloadPlayer: false, showFloorIntroText: false);
	}

	public void Play()
	{
		Scene sceneByName = SceneManager.GetSceneByName("Menu");
		if (sceneByName.IsValid() && sceneByName.isLoaded)
		{
			SceneManager.UnloadSceneAsync("Menu");
			if (floorManager.currentScene == "Menu")
			{
				floorManager.currentScene = "";
			}
		}
		if (!SaveManager.Instance.currentSave.doneTutorial2)
		{
			floorManager.UpdateRunStartTimestamp();
			floorManager.StartLoadScene("Tutorial", loadPlayer: true, loadMap: false, reloadPlayer: true, showFloorIntroText: true);
		}
		else
		{
			floorManager.StartRun();
		}
	}

	public void PauseGame(bool pauseMusic = false, GameObject pauser = null)
	{
		if (isPaused)
		{
			return;
		}
		isPaused = true;
		lastTimescale = Time.timeScale;
		Debug.Log("Game paused, last timescale: " + lastTimescale);
		Time.timeScale = 0f;
		if (MusicManager.Instance != null)
		{
			if (pauseMusic)
			{
				MusicManager.Instance.PauseEverything();
			}
			else
			{
				MusicManager.Instance.PauseSFX();
			}
		}
		if (pauser != null)
		{
			PlayerManager.Instance?.SetUIControlAuthority(pauser);
		}
		else
		{
			PlayerManager.Instance?.SetUIControlAuthority(PlayerManager.players[0]);
		}
		this.OnPause?.Invoke(isPaused);
	}

	public void UnpauseGame(bool unpauseMusic = false)
	{
		if (!isPaused)
		{
			return;
		}
		Debug.Log("Game unpaused.");
		isPaused = false;
		Time.timeScale = lastTimescale;
		if (MusicManager.Instance != null)
		{
			if (unpauseMusic)
			{
				MusicManager.Instance.UnpauseEverything();
			}
			else
			{
				MusicManager.Instance.UnpauseSFX();
			}
		}
		try
		{
			this.OnPause?.Invoke(isPaused);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in OnPause: " + ex.Message + " " + ex.StackTrace);
		}
		if (PlayerManager.Instance != null && PlayerManager.players.Length != 0 && PlayerManager.Instance.lastActivePlayerInput?.gameObject != PlayerManager.players[0])
		{
			PlayerManager.Instance.SetUIControlAuthority(PlayerManager.players[0]);
		}
	}

	public void Death()
	{
		RuntimeManager.StudioSystem.setParameterByName("InEvent", 0f);
		StartCoroutine(DeathCleanupCoroutine());
	}

	public void Restart()
	{
		RuntimeManager.StudioSystem.setParameterByName("InEvent", 0f);
		StartCoroutine(RestartCleanupCoroutine());
	}

	private IEnumerator DeathCleanupCoroutine()
	{
		CleanupAllScenes();
		CleanupRemainingObjects();
		yield return null;
		yield return new WaitForSeconds(0.1f);
		floorManager.StartRun();
	}

	private IEnumerator RestartCleanupCoroutine()
	{
		CleanupAllScenes();
		CleanupRemainingObjects();
		yield return null;
		yield return new WaitForSeconds(0.1f);
		floorManager.StartRun();
	}

	private void CleanupAllScenes()
	{
		if (SceneManager.GetSceneByName("Anarchy").IsValid())
		{
			SceneManager.UnloadSceneAsync("Anarchy");
			Debug.Log("Unloading Anarchy scene");
		}
		if (floorManager.currentScene != "")
		{
			SceneManager.UnloadSceneAsync(floorManager.currentScene);
			Debug.Log("Unloading current scene: " + floorManager.currentScene);
			floorManager.currentScene = "";
		}
		string[] array = new string[4] { "Floor 1", "Floor 2", "Floor 3", "Tutorial" };
		foreach (string text in array)
		{
			if (SceneManager.GetSceneByName(text).IsValid())
			{
				SceneManager.UnloadSceneAsync(text);
				Debug.Log("Unloading floor scene: " + text);
			}
		}
	}

	private void CleanupRemainingObjects()
	{
		Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
		Enemy[] array2 = array;
		foreach (Enemy enemy in array2)
		{
			if (enemy != null)
			{
				UnityEngine.Object.Destroy(enemy.gameObject);
			}
		}
		Debug.Log("Cleaned up " + array.Length + " enemies");
		SpellPickup[] array3 = UnityEngine.Object.FindObjectsOfType<SpellPickup>();
		SpellPickup[] array4 = array3;
		foreach (SpellPickup spellPickup in array4)
		{
			if (spellPickup != null)
			{
				UnityEngine.Object.Destroy(spellPickup.gameObject);
			}
		}
		Debug.Log("Cleaned up " + array3.Length + " spell pickups");
		HealthPickup[] array5 = UnityEngine.Object.FindObjectsOfType<HealthPickup>();
		HealthPickup[] array6 = array5;
		foreach (HealthPickup healthPickup in array6)
		{
			if (healthPickup != null)
			{
				UnityEngine.Object.Destroy(healthPickup.gameObject);
			}
		}
		Debug.Log("Cleaned up " + array5.Length + " health pickups");
		RelicPickup[] array7 = UnityEngine.Object.FindObjectsOfType<RelicPickup>();
		RelicPickup[] array8 = array7;
		foreach (RelicPickup relicPickup in array8)
		{
			if (relicPickup != null)
			{
				UnityEngine.Object.Destroy(relicPickup.gameObject);
			}
		}
		Debug.Log("Cleaned up " + array7.Length + " relic pickups");
		Health[] array9 = UnityEngine.Object.FindObjectsOfType<Health>();
		Health[] array10 = array9;
		foreach (Health health in array10)
		{
			if (health != null)
			{
				UnityEngine.Object.Destroy(health.gameObject);
			}
		}
		Debug.Log("Cleaned up " + array9.Length + " healths");
		CleanupLoadSceneAdditiveComponents();
		ResetManagers();
		GC.Collect();
	}

	private void ResetManagers()
	{
		if (WaveManager.Instance != null)
		{
			WaveManager.Instance.startedWaves = false;
			WaveManager.Instance.playerInRoom = false;
			WaveManager.Instance.currentRoom = null;
		}
		if (BossManager.Instance != null)
		{
			BossManager.Instance.EndBossFight(win: false);
		}
		if (Screenshake.Instance != null)
		{
			Screenshake.Instance.ResetTrauma();
		}
		if (global::Death.Instance != null)
		{
			global::Death.Instance.isDead = false;
		}
	}

	private void CleanupLoadSceneAdditiveComponents()
	{
		LoadSceneAdditive[] array = UnityEngine.Object.FindObjectsOfType<LoadSceneAdditive>();
		LoadSceneAdditive[] array2 = array;
		foreach (LoadSceneAdditive loadSceneAdditive in array2)
		{
			if (loadSceneAdditive != null)
			{
				UnityEngine.Object.Destroy(loadSceneAdditive.gameObject);
			}
		}
		Debug.Log("Cleaned up " + array.Length + " LoadSceneAdditive components");
	}
}
