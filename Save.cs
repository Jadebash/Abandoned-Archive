using System;
using UnityEngine;

[Serializable]
public class Save
{
	public bool developer;

	public bool doneTutorial;

	public bool doneTutorial2;

	public bool oblomeHealthInteract;

	public float volumeMaster;

	public float volumeMusic;

	public float volumeSoundEffects;

	public float menuSoundEffects;

	public string keybindOverrides;

	public bool fullscreen;

	public int resolutionIndex;

	public float uiScale;

	public float screenshakeIntensity;

	public float cursorCameraInfluence;

	public LocalisationSystem.Language language;

	public Save()
	{
		developer = false;
		doneTutorial = false;
		doneTutorial2 = false;
		oblomeHealthInteract = false;
		volumeMaster = 1f;
		volumeMusic = 1f;
		volumeSoundEffects = 1f;
		menuSoundEffects = 1f;
		keybindOverrides = "";
		fullscreen = true;
		resolutionIndex = -1;
		uiScale = 1f;
		screenshakeIntensity = 1f;
		cursorCameraInfluence = 1f;
		try
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.English:
				language = LocalisationSystem.Language.English;
				break;
			case SystemLanguage.Spanish:
				language = LocalisationSystem.Language.Spanish;
				break;
			case SystemLanguage.French:
				language = LocalisationSystem.Language.French;
				break;
			case SystemLanguage.German:
				language = LocalisationSystem.Language.German;
				break;
			case SystemLanguage.Japanese:
				language = LocalisationSystem.Language.Japanese;
				break;
			case SystemLanguage.ChineseTraditional:
				language = LocalisationSystem.Language.TraditionalChinese;
				break;
			case SystemLanguage.ChineseSimplified:
				language = LocalisationSystem.Language.SimplifiedChinese;
				break;
			case SystemLanguage.Russian:
				language = LocalisationSystem.Language.Russian;
				break;
			case SystemLanguage.Portuguese:
				language = LocalisationSystem.Language.Portuguese;
				break;
			case SystemLanguage.Polish:
				language = LocalisationSystem.Language.Polish;
				break;
			case SystemLanguage.Turkish:
				language = LocalisationSystem.Language.Turkish;
				break;
			default:
				language = LocalisationSystem.Language.English;
				break;
			}
		}
		catch
		{
			language = LocalisationSystem.Language.English;
		}
	}
}
