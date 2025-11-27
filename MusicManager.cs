using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager Instance;

	public EventInstance Music;

	private Bus musicBus;

	private Bus sfxBus;

	private string lastSong = "";

	private float originalMusicVolume = 1f;

	private void Awake()
	{
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 0f);
		Instance = this;
		musicBus = RuntimeManager.GetBus("bus:/Music");
		sfxBus = RuntimeManager.GetBus("bus:/SoundEffects");
		musicBus.getVolume(out originalMusicVolume);
	}

	public static void EnterLowIntensity()
	{
		if (FloorMusic.Instance != null && !FloorMusic.Instance.LowIntensityMusicEvent.IsNull && !(FloorMusic.Instance.LowIntensityMusicEvent.ToString() == ""))
		{
			Instance?.PlaySong(FloorMusic.Instance.LowIntensityMusicEvent);
		}
		else
		{
			Instance?.PlaySong("event:/Music/Soundtrack/Dystopia");
		}
	}

	public static void EnterMediumIntensity()
	{
		Instance?.PlaySong("event:/Music/Soundtrack/Preperation");
	}

	public static void EnterHighIntensity()
	{
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 100f);
		if (FloorMusic.Instance != null && !FloorMusic.Instance.HighIntensityMusicEvent.IsNull)
		{
			Instance?.PlaySong(FloorMusic.Instance.HighIntensityMusicEvent);
		}
		else
		{
			Instance?.PlaySong("event:/Music/Soundtrack/Enemies");
		}
	}

	public static void Calm()
	{
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 0f);
	}

	public static void EnterMenu()
	{
		RuntimeManager.StudioSystem.setParameterByName("InMenu", 1f);
	}

	public static void ExitMenu()
	{
		RuntimeManager.StudioSystem.setParameterByName("InMenu", 0f);
	}

	public void PlaySong(string customSong)
	{
		if (!(lastSong == customSong))
		{
			Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			Music = RuntimeManager.CreateInstance(customSong);
			Music.start();
			lastSong = customSong;
		}
	}

	public void PlaySong(EventReference customSong)
	{
		if (!(lastSong == customSong.ToString()))
		{
			Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			Music = RuntimeManager.CreateInstance(customSong);
			Music.start();
			lastSong = customSong.ToString();
		}
	}

	public void StopMusic()
	{
		Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		lastSong = "";
	}

	public void FadeOutAllMusic()
	{
		musicBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		lastSong = "";
	}

	public void PauseMusic()
	{
		musicBus.setPaused(paused: true);
	}

	public void UnpauseMusic()
	{
		musicBus.setPaused(paused: false);
	}

	public void PauseSFX()
	{
		sfxBus.setPaused(paused: true);
	}

	public void StopSFX()
	{
		sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void UnpauseSFX()
	{
		sfxBus.setPaused(paused: false);
	}

	public void PauseEverything()
	{
		musicBus.setPaused(paused: true);
		sfxBus.setPaused(paused: true);
	}

	public void UnpauseEverything()
	{
		musicBus.setPaused(paused: false);
		sfxBus.setPaused(paused: false);
	}

	public void SetMusicVolume(float volumeMultiplier)
	{
		musicBus.setVolume(SaveManager.Instance.currentSave.volumeMusic * volumeMultiplier);
	}
}
