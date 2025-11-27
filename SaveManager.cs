using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
	public static SaveManager Instance;

	private Save _currentSave;

	public Save currentSave
	{
		get
		{
			if (_currentSave == null)
			{
				_currentSave = SaveSystem.LoadGame();
				if (_currentSave == null)
				{
					currentSave = new Save();
				}
			}
			return _currentSave;
		}
		set
		{
			_currentSave = value;
			SaveSystem.SaveGame(_currentSave);
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		LoadSaveSettings();
	}

	public void LoadSaveSettings()
	{
		Debug.Log("Loading save settings");
		RuntimeManager.GetBus("bus:/").setVolume(currentSave.volumeMaster);
		RuntimeManager.GetBus("bus:/Music").setVolume(currentSave.volumeMusic);
		RuntimeManager.GetBus("bus:/SoundEffects").setVolume(currentSave.volumeSoundEffects);
		RuntimeManager.GetBus("bus:/UI").setVolume(currentSave.menuSoundEffects);
		PlayerInput[] array = Object.FindObjectsOfType<PlayerInput>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].actions.LoadBindingOverridesFromJson(currentSave.keybindOverrides);
			UIControl.UpdateAllTooltips();
		}
		Screen.fullScreen = currentSave.fullscreen;
		LocalisationSystem.language = currentSave.language;
		UIScaling.BroadcastUIScaleChanged();
		if (currentSave.resolutionIndex != -1 && currentSave.resolutionIndex < Screen.resolutions.Length)
		{
			Screen.SetResolution(Screen.resolutions[currentSave.resolutionIndex].width, Screen.resolutions[currentSave.resolutionIndex].height, currentSave.fullscreen, Screen.resolutions[currentSave.resolutionIndex].refreshRate);
		}
		else if (Steam.Instance != null && Steam.Instance.IsSteamDeck())
		{
			bool flag = false;
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				if (resolution.width == 1280 && resolution.height == 800)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Screen.SetResolution(1280, 800, fullscreen: true);
			}
			else
			{
				Screen.SetResolution(1280, 720, fullscreen: true);
			}
		}
		if (currentSave.doneTutorial2)
		{
			Steam.TriggerAchievement("ACH_FINISH_TUTORIAL");
		}
	}

	public void SetTutorialComplete()
	{
		Save save = currentSave;
		currentSave.doneTutorial2 = true;
		currentSave = save;
	}

	public void ResetTutorialState()
	{
		Save save = currentSave;
		save.doneTutorial = false;
		save.doneTutorial2 = false;
		currentSave = save;
	}

	public void ResetSaveFile()
	{
		SaveSystem.SaveGame(new Save());
	}

	public void SetLanguage(LocalisationSystem.Language language)
	{
		Save save = currentSave;
		save.language = language;
		currentSave = save;
	}
}
