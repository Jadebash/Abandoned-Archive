using System;
using UnityEngine;

[Serializable]
public class VideoSettings
{
	public int resolutionIndex;

	public bool isFullscreen;

	public static VideoSettings CreateFromCurrentScreen()
	{
		Resolution[] resolutions = Screen.resolutions;
		int num = 0;
		for (int i = 0; i < resolutions.Length; i++)
		{
			if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
			{
				num = i;
				break;
			}
		}
		return new VideoSettings
		{
			resolutionIndex = num,
			isFullscreen = Screen.fullScreen
		};
	}

	public VideoSettings Clone()
	{
		return new VideoSettings
		{
			resolutionIndex = resolutionIndex,
			isFullscreen = isFullscreen
		};
	}

	public void Apply()
	{
		Resolution[] resolutions = Screen.resolutions;
		if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
		{
			Resolution resolution = resolutions[resolutionIndex];
			Screen.SetResolution(resolution.width, resolution.height, isFullscreen, resolution.refreshRate);
		}
		else
		{
			Screen.fullScreen = isFullscreen;
		}
	}

	public void SaveToDisk()
	{
		if (SaveManager.Instance != null)
		{
			Save currentSave = SaveManager.Instance.currentSave;
			currentSave.resolutionIndex = resolutionIndex;
			currentSave.fullscreen = isFullscreen;
			SaveManager.Instance.currentSave = currentSave;
			Debug.Log("Video settings saved to disk. Resolution: " + resolutionIndex + ", Fullscreen: " + isFullscreen);
		}
		else
		{
			Debug.LogWarning("Cannot save video settings - SaveManager.Instance is null");
		}
	}

	public bool IsDifferentFrom(VideoSettings other)
	{
		if (other == null)
		{
			return true;
		}
		if (resolutionIndex == other.resolutionIndex)
		{
			return isFullscreen != other.isFullscreen;
		}
		return true;
	}
}
