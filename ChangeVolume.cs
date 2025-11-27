using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour
{
	private Bus bus;

	public AudioBus busType;

	public bool updateSliderOnEnableAndStart = true;

	private float currentVolume;

	private void Start()
	{
		switch (busType)
		{
		case AudioBus.Master:
			bus = RuntimeManager.GetBus("bus:/");
			break;
		case AudioBus.Music:
			bus = RuntimeManager.GetBus("bus:/Music");
			break;
		case AudioBus.SoundEffects:
			bus = RuntimeManager.GetBus("bus:/SoundEffects");
			break;
		case AudioBus.MenuSounds:
			bus = RuntimeManager.GetBus("bus:/UI");
			break;
		default:
			bus = RuntimeManager.GetBus("bus:/");
			break;
		}
		UpdateSlider();
	}

	private void OnEnable()
	{
		UpdateSlider();
	}

	private void UpdateSlider()
	{
		float num = 0f;
		if (SaveManager.Instance != null)
		{
			switch (busType)
			{
			case AudioBus.Master:
				num = SaveManager.Instance.currentSave.volumeMaster;
				break;
			case AudioBus.Music:
				num = SaveManager.Instance.currentSave.volumeMusic;
				break;
			case AudioBus.SoundEffects:
				num = SaveManager.Instance.currentSave.volumeSoundEffects;
				break;
			case AudioBus.MenuSounds:
				num = SaveManager.Instance.currentSave.menuSoundEffects;
				break;
			}
			UpdateVolume(num);
		}
		else
		{
			Debug.LogWarning("No SaveManager found.");
		}
		currentVolume = num;
		GetComponent<Slider>().value = num;
	}

	public void UpdateVolume(float newVolume)
	{
		bus.setVolume(newVolume);
		currentVolume = newVolume;
	}

	private void OnDisable()
	{
		SaveVolume();
	}

	private void OnDestroy()
	{
		SaveVolume();
	}

	private void SaveVolume()
	{
		if (SaveManager.Instance != null)
		{
			Save currentSave = SaveManager.Instance.currentSave;
			switch (busType)
			{
			case AudioBus.Master:
				SaveManager.Instance.currentSave.volumeMaster = currentVolume;
				break;
			case AudioBus.Music:
				SaveManager.Instance.currentSave.volumeMusic = currentVolume;
				break;
			case AudioBus.SoundEffects:
				SaveManager.Instance.currentSave.volumeSoundEffects = currentVolume;
				break;
			case AudioBus.MenuSounds:
				SaveManager.Instance.currentSave.menuSoundEffects = currentVolume;
				break;
			}
			SaveManager.Instance.currentSave = currentSave;
		}
	}
}
