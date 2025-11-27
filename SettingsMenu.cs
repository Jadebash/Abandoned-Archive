using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	private Resolution[] resolutions;

	[Header("Video Settings UI")]
	public Dropdown resolutionDropdown;

	public Dropdown displayModeDropdown;

	public VideoSettingsConfirmation videoSettingsConfirmation;

	public GameObject videoSettingsMenu;

	public Button backButton;

	private VideoSettings currentSettings;

	private bool isInitializing = true;

	private Coroutine dropdownMonitorCoroutine;

	private Coroutine selectionMonitorCoroutine;

	private GameObject lastSelectedObject;

	private ScrollRect currentScrollRect;

	private Transform currentContent;

	private void Start()
	{
		isInitializing = true;
		resolutions = Screen.resolutions;
		InitializeResolutionDropdown();
		InitializeDisplayModeDropdown();
		SetupDropdownHoverScrolling();
		if (videoSettingsConfirmation != null)
		{
			videoSettingsConfirmation.OnConfirmed += OnVideoSettingsConfirmed;
			videoSettingsConfirmation.OnReverted += OnVideoSettingsReverted;
		}
		if (currentSettings != null)
		{
			UpdateUIFromSettings(currentSettings);
		}
		isInitializing = false;
	}

	private void OnEnable()
	{
		if (!isInitializing && resolutions != null && resolutions.Length != 0)
		{
			RefreshSettingsFromSaveOrScreen();
		}
	}

	private void Awake()
	{
		LoadAndApplySavedSettings();
	}

	private void OnDestroy()
	{
		if (videoSettingsConfirmation != null)
		{
			videoSettingsConfirmation.OnConfirmed -= OnVideoSettingsConfirmed;
			videoSettingsConfirmation.OnReverted -= OnVideoSettingsReverted;
		}
		if (dropdownMonitorCoroutine != null)
		{
			StopCoroutine(dropdownMonitorCoroutine);
		}
		if (selectionMonitorCoroutine != null)
		{
			StopCoroutine(selectionMonitorCoroutine);
		}
	}

	private void InitializeResolutionDropdown()
	{
		resolutionDropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < resolutions.Length; i++)
		{
			string item = resolutions[i].width + "x" + resolutions[i].height + "(" + resolutions[i].refreshRate + "hz)";
			list.Add(item);
		}
		resolutionDropdown.AddOptions(list);
	}

	private void InitializeDisplayModeDropdown()
	{
		displayModeDropdown.ClearOptions();
		List<string> list = new List<string>();
		list.Add("Fullscreen");
		list.Add("Windowed");
		displayModeDropdown.AddOptions(list);
	}

	private void LoadAndApplySavedSettings()
	{
		if (SaveManager.Instance != null && SaveManager.Instance.currentSave.resolutionIndex > -1)
		{
			VideoSettings videoSettings = new VideoSettings
			{
				resolutionIndex = SaveManager.Instance.currentSave.resolutionIndex,
				isFullscreen = SaveManager.Instance.currentSave.fullscreen
			};
			videoSettings.Apply();
			currentSettings = videoSettings;
		}
		else
		{
			currentSettings = VideoSettings.CreateFromCurrentScreen();
		}
	}

	private void RefreshSettingsFromSaveOrScreen()
	{
		if (SaveManager.Instance != null && SaveManager.Instance.currentSave.resolutionIndex > -1)
		{
			currentSettings = new VideoSettings
			{
				resolutionIndex = SaveManager.Instance.currentSave.resolutionIndex,
				isFullscreen = SaveManager.Instance.currentSave.fullscreen
			};
			currentSettings.Apply();
		}
		else
		{
			currentSettings = VideoSettings.CreateFromCurrentScreen();
		}
		UpdateUIFromSettings(currentSettings);
	}

	private void UpdateUIFromSettings(VideoSettings settings)
	{
		if (settings != null)
		{
			resolutionDropdown.SetValueWithoutNotify(settings.resolutionIndex);
			resolutionDropdown.RefreshShownValue();
			displayModeDropdown.SetValueWithoutNotify((!settings.isFullscreen) ? 1 : 0);
			displayModeDropdown.RefreshShownValue();
		}
	}

	public void SetResolution(int resolutionIndex)
	{
		if (!isInitializing)
		{
			VideoSettings videoSettings = currentSettings.Clone();
			videoSettings.resolutionIndex = resolutionIndex;
			ApplyVideoSettingsWithConfirmation(videoSettings);
		}
	}

	public void SetDisplayMode(int displayModeIndex)
	{
		if (!isInitializing)
		{
			bool isFullscreen = displayModeIndex == 0;
			VideoSettings videoSettings = currentSettings.Clone();
			videoSettings.isFullscreen = isFullscreen;
			ApplyVideoSettingsWithConfirmation(videoSettings);
		}
	}

	private void ApplyVideoSettingsWithConfirmation(VideoSettings newSettings)
	{
		if (newSettings.IsDifferentFrom(currentSettings))
		{
			newSettings.Apply();
			if (videoSettingsConfirmation != null && videoSettingsMenu != null && videoSettingsMenu.activeSelf)
			{
				videoSettingsConfirmation.ShowConfirmation(currentSettings, newSettings);
				return;
			}
			newSettings.SaveToDisk();
			currentSettings = newSettings;
		}
	}

	private void OnVideoSettingsConfirmed()
	{
		if (SaveManager.Instance != null && SaveManager.Instance.currentSave.resolutionIndex > -1)
		{
			currentSettings = new VideoSettings
			{
				resolutionIndex = SaveManager.Instance.currentSave.resolutionIndex,
				isFullscreen = SaveManager.Instance.currentSave.fullscreen
			};
		}
		else
		{
			currentSettings = VideoSettings.CreateFromCurrentScreen();
		}
		UpdateUIFromSettings(currentSettings);
		StartCoroutine(SelectBackButtonDelayed());
	}

	private void OnVideoSettingsReverted()
	{
		UpdateUIFromSettings(currentSettings);
		StartCoroutine(SelectBackButtonDelayed());
	}

	private IEnumerator SelectBackButtonDelayed()
	{
		yield return null;
		if (!(EventSystem.current != null))
		{
			yield break;
		}
		if (backButton != null && backButton.gameObject.activeInHierarchy)
		{
			backButton.Select();
			yield break;
		}
		GameObject gameObject = GameObject.Find("BackButton");
		if (gameObject != null && gameObject.activeInHierarchy)
		{
			Button component = gameObject.GetComponent<Button>();
			if (component != null)
			{
				component.Select();
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}
	}

	[Obsolete("Use SetDisplayMode instead")]
	public void SetFullScreen()
	{
		SetDisplayMode(0);
	}

	[Obsolete("Use SetDisplayMode instead")]
	public void SetWindowed()
	{
		SetDisplayMode(1);
	}

	public void SetLanguage(int languageIndex)
	{
		switch (languageIndex)
		{
		case 0:
			LocalisationSystem.language = LocalisationSystem.Language.English;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.English);
			break;
		case 1:
			LocalisationSystem.language = LocalisationSystem.Language.Spanish;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.Spanish);
			break;
		case 2:
			LocalisationSystem.language = LocalisationSystem.Language.French;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.French);
			break;
		case 3:
			LocalisationSystem.language = LocalisationSystem.Language.German;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.German);
			break;
		case 4:
			LocalisationSystem.language = LocalisationSystem.Language.Japanese;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.Japanese);
			break;
		case 5:
			LocalisationSystem.language = LocalisationSystem.Language.TraditionalChinese;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.TraditionalChinese);
			break;
		case 6:
			LocalisationSystem.language = LocalisationSystem.Language.SimplifiedChinese;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.SimplifiedChinese);
			break;
		case 7:
			LocalisationSystem.language = LocalisationSystem.Language.Portuguese;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.Portuguese);
			break;
		case 8:
			LocalisationSystem.language = LocalisationSystem.Language.Polish;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.Polish);
			break;
		case 9:
			LocalisationSystem.language = LocalisationSystem.Language.Turkish;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.Turkish);
			break;
		default:
			LocalisationSystem.language = LocalisationSystem.Language.English;
			SaveManager.Instance.SetLanguage(LocalisationSystem.Language.English);
			break;
		}
	}

	public void Exit()
	{
		SceneManager.UnloadSceneAsync("Settings");
	}

	public void RestartAudio()
	{
		MusicManager.ExitMenu();
	}

	private void SetupDropdownHoverScrolling()
	{
		if (resolutionDropdown != null)
		{
			dropdownMonitorCoroutine = StartCoroutine(MonitorDropdownList());
		}
	}

	private IEnumerator MonitorDropdownList()
	{
		while (true)
		{
			yield return null;
			GameObject dropdownList = GameObject.Find("Dropdown List");
			if (dropdownList != null && dropdownList.activeInHierarchy)
			{
				yield return null;
				SetupHoverScrollingForDropdownList(dropdownList);
				if (selectionMonitorCoroutine != null)
				{
					StopCoroutine(selectionMonitorCoroutine);
				}
				selectionMonitorCoroutine = StartCoroutine(MonitorSelectionChanges(dropdownList));
				while (dropdownList != null && dropdownList.activeInHierarchy)
				{
					yield return null;
				}
				if (selectionMonitorCoroutine != null)
				{
					StopCoroutine(selectionMonitorCoroutine);
					selectionMonitorCoroutine = null;
				}
				lastSelectedObject = null;
				currentScrollRect = null;
				currentContent = null;
			}
		}
	}

	private void SetupHoverScrollingForDropdownList(GameObject dropdownList)
	{
		ScrollRect scrollRect = dropdownList.GetComponentInChildren<ScrollRect>();
		if (scrollRect == null)
		{
			return;
		}
		currentScrollRect = scrollRect;
		Transform content = scrollRect.content;
		if (content == null)
		{
			return;
		}
		currentContent = content;
		Toggle[] componentsInChildren = content.GetComponentsInChildren<Toggle>();
		foreach (Toggle toggle in componentsInChildren)
		{
			EventTrigger eventTrigger = toggle.gameObject.GetComponent<EventTrigger>();
			if (eventTrigger == null)
			{
				eventTrigger = toggle.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.Entry entry = eventTrigger.triggers.Find((EventTrigger.Entry e) => e.eventID == EventTriggerType.PointerEnter);
			if (entry != null)
			{
				eventTrigger.triggers.Remove(entry);
			}
			EventTrigger.Entry entry2 = eventTrigger.triggers.Find((EventTrigger.Entry e) => e.eventID == EventTriggerType.Select);
			if (entry2 != null)
			{
				eventTrigger.triggers.Remove(entry2);
			}
			EventTrigger.Entry entry3 = new EventTrigger.Entry();
			entry3.eventID = EventTriggerType.PointerEnter;
			entry3.callback.AddListener(delegate
			{
				ScrollToMakeVisible(toggle.gameObject, scrollRect, content);
			});
			eventTrigger.triggers.Add(entry3);
			EventTrigger.Entry entry4 = new EventTrigger.Entry();
			entry4.eventID = EventTriggerType.Select;
			entry4.callback.AddListener(delegate
			{
				ScrollToMakeVisible(toggle.gameObject, scrollRect, content);
			});
			eventTrigger.triggers.Add(entry4);
		}
	}

	private void ScrollToMakeVisible(GameObject item, ScrollRect scrollRect, Transform content)
	{
		if (!(item == null) && !(scrollRect == null) && !(content == null))
		{
			RectTransform component = item.GetComponent<RectTransform>();
			RectTransform viewport = scrollRect.viewport;
			RectTransform component2 = content.GetComponent<RectTransform>();
			if (!(component == null) && !(viewport == null) && !(component2 == null))
			{
				StartCoroutine(ScrollToItemCoroutine(component, scrollRect, viewport, component2));
			}
		}
	}

	private IEnumerator ScrollToItemCoroutine(RectTransform itemRect, ScrollRect scrollRect, RectTransform viewportRect, RectTransform contentRect)
	{
		yield return new WaitForEndOfFrame();
		Canvas.ForceUpdateCanvases();
		Vector3[] array = new Vector3[4];
		itemRect.GetWorldCorners(array);
		Vector3[] array2 = new Vector3[4];
		viewportRect.GetWorldCorners(array2);
		float y = array[1].y;
		float y2 = array[0].y;
		float y3 = array2[1].y;
		float y4 = array2[0].y;
		if (y <= y3 && y2 >= y4)
		{
			yield break;
		}
		float height = contentRect.rect.height;
		float height2 = viewportRect.rect.height;
		float num = Mathf.Max(0f, height - height2);
		if (num > 0f)
		{
			float value = scrollRect.verticalNormalizedPosition;
			if (y > y3)
			{
				Vector3 position = array[1];
				float num2 = 0f - contentRect.InverseTransformPoint(position).y;
				value = 1f - num2 / num;
			}
			else if (y2 < y4)
			{
				Vector3 position2 = array[0];
				float num3 = 0f - contentRect.InverseTransformPoint(position2).y - height2;
				value = 1f - num3 / num;
			}
			value = Mathf.Clamp01(value);
			scrollRect.verticalNormalizedPosition = value;
		}
	}

	private IEnumerator MonitorSelectionChanges(GameObject dropdownList)
	{
		while (dropdownList != null && dropdownList.activeInHierarchy && currentScrollRect != null && currentContent != null)
		{
			yield return null;
			if (EventSystem.current != null)
			{
				GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
				if (currentSelectedGameObject != null && currentSelectedGameObject != lastSelectedObject && currentSelectedGameObject.transform.IsChildOf(dropdownList.transform) && currentSelectedGameObject.GetComponent<Toggle>() != null)
				{
					ScrollToMakeVisible(currentSelectedGameObject, currentScrollRect, currentContent);
					lastSelectedObject = currentSelectedGameObject;
				}
			}
		}
	}
}
