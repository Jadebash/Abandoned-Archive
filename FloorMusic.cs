using FMODUnity;
using UnityEngine;

public class FloorMusic : MonoBehaviour
{
	public static FloorMusic Instance;

	public bool playLowIntensity = true;

	public bool playHighIntensity;

	public EventReference HighIntensityMusicEvent;

	public EventReference LowIntensityMusicEvent;

	private void Awake()
	{
		Instance = this;
		RuntimeManager.StudioSystem.setParameterByName("Intensity", 0f, ignoreseekspeed: true);
		MusicManager.Instance?.PlaySong(Instance.LowIntensityMusicEvent);
	}
}
