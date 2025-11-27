using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FloorManager : MonoBehaviour
{
	public delegate void StartFloor(int floorNumber);

	public delegate void CompleteFloor(int floorNumber);

	public delegate void FinishedLoading();

	public static FloorManager Instance;

	public List<Floor> floors = new List<Floor>();

	public GameObject loadingScreen;

	public GameObject loadingCamera;

	public CanvasGroup loadingAlpha;

	public CanvasGroup loadingUIAlpha;

	public Slider loadingBar;

	private int runStartTimestamp;

	[HideInInspector]
	public string currentScene = "";

	[HideInInspector]
	public int currentFloor = 1;

	[HideInInspector]
	public int currentLoop = 1;

	[HideInInspector]
	public bool playerSceneLoaded;

	private bool loadingScene;

	[HideInInspector]
	public bool isLoadingFloor;

	[HideInInspector]
	public int roomsClearedThisRun;

	[HideInInspector]
	public string startingSpellId = "";

	public LocalisedString tutorialName;

	public Dialogue tutorialExitDialogue;

	private bool playTutorialExitDialogue;

	private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

	private float totalSceneProgress;

	private float totalGenerationProgress = 100f;

	private Coroutine fadeOutCoroutine;

	public event StartFloor OnStartFloor;

	public event CompleteFloor OnCompleteFloor;

	public event FinishedLoading OnFinishedLoading;

	private void Awake()
	{
		Instance = this;
	}

	public void QueueTutorialExitDialogue()
	{
		playTutorialExitDialogue = true;
	}

	public void StartRun()
	{
		if (PlayerManager.Instance != null)
		{
			PlayerManager.Instance.ClearPlayers();
		}
		UpdateRunStartTimestamp();
		if (roomsClearedThisRun >= 2)
		{
			startingSpellId = "";
		}
		roomsClearedThisRun = 0;
		currentLoop = 1;
		LoadFloor(1, loadPlayer: true, forceShowIntro: true);
	}

	public Floor GetFloor(int floorNumber)
	{
		if (floorNumber == 0)
		{
			Debug.LogWarning("Floor number passed in GetFloor is zero.");
			return null;
		}
		return floors[floorNumber - 1];
	}

	public string GetFloorNameLower()
	{
		return SceneManager.GetActiveScene().name.ToLower();
	}

	public int UpdateRunStartTimestamp()
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		runStartTimestamp = (int)(DateTime.UtcNow - dateTime).TotalSeconds;
		return runStartTimestamp;
	}

	public void CompletedCurrentFloor()
	{
		this.OnCompleteFloor?.Invoke(currentFloor);
	}

	public void NextFloor()
	{
		if (SceneManager.GetActiveScene().name.ToUpper() == "TUTORIAL")
		{
			return;
		}
		if (loadingScene)
		{
			Debug.LogWarning("Cannot load next floor, already loading one.");
			return;
		}
		if (RelicCollector.Instance != null)
		{
			RelicCollector.Instance.RemoveFloorBoundRelics();
		}
		currentFloor++;
		if (currentFloor > floors.Count)
		{
			currentFloor = 1;
			currentLoop++;
			Debug.Log("Looping: " + currentLoop);
		}
		SceneManager.GetSceneByName(GetFloor(currentFloor).sceneName);
		LoadFloor(currentFloor, loadPlayer: false, forceShowIntro: true);
	}

	public void LoadFloor(int floorNumber, bool loadPlayer = true, bool forceShowIntro = false)
	{
		LocalisationSystem.Language language = LocalisationSystem.language;
		LocalisationSystem.language = LocalisationSystem.Language.English;
		currentFloor = floorNumber;
		StartLoadScene(showFloorIntroText: currentFloor != 1 || forceShowIntro, scene: GetFloor(floorNumber).sceneName, loadPlayer: loadPlayer, loadMap: true, reloadPlayer: loadPlayer);
		this.OnStartFloor?.Invoke(floorNumber);
		LocalisationSystem.language = language;
	}

	public void UnloadPlayer()
	{
		LocalisationSystem.Language language = LocalisationSystem.language;
		LocalisationSystem.language = LocalisationSystem.Language.English;
		if (playerSceneLoaded)
		{
			SceneManager.UnloadSceneAsync("Player");
			playerSceneLoaded = false;
		}
		LocalisationSystem.language = language;
	}

	public void StartLoadScene(string scene, bool loadPlayer, bool loadMap, bool reloadPlayer, bool showFloorIntroText)
	{
		LocalisationSystem.Language language = LocalisationSystem.language;
		LocalisationSystem.language = LocalisationSystem.Language.English;
		if (loadingScene)
		{
			Debug.LogWarning("Cannot load scene, already loading one.");
			return;
		}
		if (fadeOutCoroutine != null)
		{
			StopCoroutine(fadeOutCoroutine);
		}
		loadingScene = true;
		StartCoroutine(LoadScene(scene, loadPlayer, loadMap, reloadPlayer, showFloorIntroText));
		LocalisationSystem.language = language;
	}

	private IEnumerator LoadScene(string scene, bool loadPlayer, bool loadMap, bool reloadPlayer, bool showFloorIntroText)
	{
		isLoadingFloor = true;
		yield return FadeInLoading();
		if (currentScene != "")
		{
			scenesLoading.Add(SceneManager.UnloadSceneAsync(currentScene));
		}
		if (reloadPlayer)
		{
			UnloadPlayer();
		}
		if (loadPlayer && reloadPlayer)
		{
			scenesLoading.Add(SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive));
			playerSceneLoaded = true;
		}
		scenesLoading.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
		currentScene = scene;
		StartCoroutine(GetSceneLoadingProgress(scene));
		if (loadMap)
		{
			StartCoroutine(GetGenerationProgress());
		}
		StartCoroutine(GetTotalProgress(showFloorIntroText));
		string text = scene;
		if (text.Contains("Floor"))
		{
			text = GetCurrentFloorName();
		}
		DiscordController.Instance.UpdatePresence("In " + text, runStartTimestamp);
	}

	public IEnumerator GetSceneLoadingProgress(string scene)
	{
		for (int i = 0; i < scenesLoading.Count; i++)
		{
			while (!scenesLoading[i].isDone)
			{
				totalSceneProgress = 0f;
				foreach (AsyncOperation item in scenesLoading)
				{
					totalSceneProgress += item.progress;
				}
				totalSceneProgress = totalSceneProgress / (float)scenesLoading.Count * 100f;
				yield return null;
			}
		}
		totalSceneProgress = 100f;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
	}

	public IEnumerator GetGenerationProgress()
	{
		while (MapGenerator.Instance == null || MapGenerator.Instance.generationProgress != 100f)
		{
			if (MapGenerator.Instance == null)
			{
				totalGenerationProgress = 0f;
			}
			else
			{
				totalGenerationProgress = MapGenerator.Instance.generationProgress;
			}
			yield return null;
		}
		totalGenerationProgress = 100f;
	}

	public string GetCurrentFloorName()
	{
		return GetFloor(currentFloor).floorName.value.Replace("[[NUM]]", (currentFloor + (currentLoop - 1) * floors.Count).ToString());
	}

	public IEnumerator GetTotalProgress(bool showFloorIntroText)
	{
		float totalProgress = 0f;
		while (totalProgress != 100f)
		{
			totalProgress = Mathf.Round((totalSceneProgress + totalGenerationProgress) / 2f);
			if (loadingBar != null)
			{
				loadingBar.value = Mathf.RoundToInt(totalProgress);
			}
			yield return null;
		}
		loadingScene = false;
		fadeOutCoroutine = StartCoroutine(FadeOutLoading());
		Bus bus = RuntimeManager.GetBus("bus:/SoundEffects");
		if (SaveManager.Instance != null)
		{
			bus.setVolume(SaveManager.Instance.currentSave.volumeSoundEffects);
		}
		else
		{
			bus.setVolume(1f);
		}
		if (showFloorIntroText)
		{
			if (currentScene.Equals("Tutorial", StringComparison.OrdinalIgnoreCase))
			{
				RunManager.Instance?.ShowFloorIntroText(GetFloor(currentFloor).placeName.value, tutorialName.value);
			}
			else
			{
				RunManager.Instance?.ShowFloorIntroText(GetFloor(currentFloor).placeName.value, GetCurrentFloorName());
			}
		}
		ResetPlayerPositionAfterTeleport();
		isLoadingFloor = false;
		this.OnFinishedLoading?.Invoke();
		if (playTutorialExitDialogue && currentFloor == 1 && DialogueSystem.Instance != null)
		{
			DialogueSystem.Instance.StartDialogue(tutorialExitDialogue);
			playTutorialExitDialogue = false;
		}
	}

	public IEnumerator FadeInLoading()
	{
		loadingScreen.SetActive(value: true);
		loadingBar.value = 0f;
		if (SceneManager.GetActiveScene().name == "Manager")
		{
			loadingAlpha.alpha = 1f;
			loadingUIAlpha.alpha = 1f;
			loadingCamera.SetActive(value: true);
			yield return null;
			yield break;
		}
		if (Camera.main == null)
		{
			loadingCamera.SetActive(value: true);
		}
		float duration = 1f;
		float startTime = Time.time;
		while (Time.time < startTime + duration && loadingAlpha.alpha != 1f)
		{
			loadingAlpha.alpha += Time.unscaledDeltaTime / duration;
			yield return null;
		}
		float durationUI = 0.5f;
		float startTimeUI = Time.time;
		while (Time.time < startTimeUI + durationUI && loadingUIAlpha.alpha != 1f)
		{
			loadingUIAlpha.alpha += Time.unscaledDeltaTime / durationUI;
			yield return null;
		}
		loadingAlpha.alpha = 1f;
		loadingUIAlpha.alpha = 1f;
		if (!Manager.Instance.isPaused)
		{
			Time.timeScale = 1f;
		}
		loadingCamera.SetActive(value: true);
	}

	public IEnumerator FadeOutLoading(bool noUI = false)
	{
		loadingScreen.SetActive(value: true);
		loadingCamera.SetActive(value: false);
		loadingAlpha.alpha = 1f;
		loadingUIAlpha.alpha = 1f;
		if (noUI)
		{
			loadingUIAlpha.alpha = 0f;
		}
		if (Manager.Instance.isPaused && Tutorial.Instance == null)
		{
			Manager.Instance.UnpauseGame(unpauseMusic: true);
		}
		if (Tutorial.Instance == null)
		{
			Time.timeScale = 1f;
		}
		float durationUI = 0.5f;
		float startTimeUI = Time.time;
		while (Time.time < startTimeUI + durationUI && loadingUIAlpha.alpha != 0f)
		{
			loadingUIAlpha.alpha -= Time.unscaledDeltaTime / durationUI;
			yield return null;
		}
		float duration = 1f;
		float startTime = Time.time;
		while (Time.time < startTime + duration && loadingAlpha.alpha != 0f)
		{
			loadingAlpha.alpha -= Time.unscaledDeltaTime / duration;
			yield return null;
		}
		loadingUIAlpha.alpha = 0f;
		loadingAlpha.alpha = 0f;
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject != null)
			{
				Movement component = gameObject.GetComponent<Movement>();
				if (component != null)
				{
					component.isTeleporting = false;
				}
			}
		}
		loadingScreen.SetActive(value: false);
	}

	public void PlayedThroughEditor(string newCurrentScene = "", int newCurrentFloor = 1)
	{
		currentFloor = newCurrentFloor;
		currentScene = newCurrentScene;
		StartCoroutine(FadeOutLoading(noUI: true));
		if (newCurrentScene.Equals("Menu", StringComparison.OrdinalIgnoreCase))
		{
			DiscordController.Instance.UpdatePresence("In Menu", UpdateRunStartTimestamp());
			return;
		}
		Debug.Log("Played through editor at " + newCurrentScene);
		if (newCurrentScene.Equals("Tutorial", StringComparison.OrdinalIgnoreCase))
		{
			DiscordController.Instance.UpdatePresence("In Tutorial", UpdateRunStartTimestamp());
			RunManager.Instance?.ShowFloorIntroText(GetFloor(currentFloor).placeName.value, "Tutorial");
		}
		else if (newCurrentScene.Equals("Hub", StringComparison.OrdinalIgnoreCase))
		{
			DiscordController.Instance.UpdatePresence("In Hub", UpdateRunStartTimestamp());
			RunManager.Instance?.ShowFloorIntroText(GetFloor(currentFloor).placeName.value, "Hub");
		}
		else if (newCurrentScene.Equals("Anarchy", StringComparison.OrdinalIgnoreCase))
		{
			DiscordController.Instance.UpdatePresence("In Anarchy", UpdateRunStartTimestamp());
			RunManager.Instance?.ShowFloorIntroText(GetFloor(currentFloor).placeName.value, "Anarchy");
		}
		else
		{
			DiscordController.Instance.UpdatePresence("In Floor " + currentFloor, UpdateRunStartTimestamp());
			RunManager.Instance?.ShowFloorIntroText(GetFloor(currentFloor).placeName.value, GetCurrentFloorName());
		}
		if (GameObject.FindGameObjectWithTag("Player") != null)
		{
			playerSceneLoaded = true;
			this.OnStartFloor?.Invoke(currentFloor);
			this.OnFinishedLoading?.Invoke();
		}
		FloorMusic floorMusic = UnityEngine.Object.FindObjectOfType<FloorMusic>();
		if (floorMusic != null && floorMusic.playLowIntensity)
		{
			MusicManager.EnterLowIntensity();
		}
		if (floorMusic != null && floorMusic.playHighIntensity)
		{
			MusicManager.EnterHighIntensity();
		}
	}

	private void ResetPlayerPositionAfterTeleport()
	{
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			Movement component = gameObject.GetComponent<Movement>();
			if (component != null)
			{
				component.SafeTeleport(new Vector3(0f, 0f, 0f));
			}
			Transform transform = gameObject.transform.Find("Model");
			if (transform != null)
			{
				SpriteRenderer component2 = transform.GetComponent<SpriteRenderer>();
				if (component2 != null && component2.material != null)
				{
					component2.material.SetFloat("_Dissolve", 0f);
				}
			}
		}
		if (CameraController.Instance != null)
		{
			CameraController.Instance.transform.position = new Vector3(0f, 0f, 0f);
			CameraController.Instance.Unlock();
		}
	}
}
