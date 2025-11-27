using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VideoSettingsConfirmation : MonoBehaviour
{
	public static VideoSettingsConfirmation Instance;

	[Header("UI References")]
	public GameObject confirmationPanel;

	public TextMeshProUGUI countdownText;

	public Button confirmButton;

	public Button revertButton;

	[Header("Settings")]
	[Tooltip("Time in seconds before automatically reverting changes")]
	public float revertTime = 10f;

	private Coroutine countdownCoroutine;

	private VideoSettings previousSettings;

	private VideoSettings newSettings;

	public event Action OnConfirmed;

	public event Action OnReverted;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			if (confirmationPanel != null)
			{
				confirmationPanel.SetActive(value: false);
			}
			if (confirmButton != null)
			{
				confirmButton.onClick.AddListener(ConfirmChanges);
			}
			if (revertButton != null)
			{
				revertButton.onClick.AddListener(RevertChanges);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void ShowConfirmation(VideoSettings previousSettings, VideoSettings newSettings)
	{
		this.previousSettings = previousSettings?.Clone();
		this.newSettings = newSettings?.Clone();
		if (countdownCoroutine != null)
		{
			StopCoroutine(countdownCoroutine);
		}
		if (confirmationPanel != null)
		{
			confirmationPanel.SetActive(value: true);
		}
		StartCoroutine(SelectRevertButtonDelayed());
		countdownCoroutine = StartCoroutine(CountdownTimer());
	}

	private IEnumerator SelectRevertButtonDelayed()
	{
		yield return null;
		if (revertButton != null && revertButton.gameObject.activeInHierarchy)
		{
			revertButton.Select();
		}
		else if (EventSystem.current != null && revertButton != null)
		{
			EventSystem.current.SetSelectedGameObject(revertButton.gameObject);
		}
	}

	private IEnumerator CountdownTimer()
	{
		float timeRemaining = revertTime;
		if (countdownText != null)
		{
			_ = countdownText.text;
		}
		while (timeRemaining > 0f)
		{
			if (countdownText != null)
			{
				countdownText.text = Mathf.CeilToInt(timeRemaining).ToString();
			}
			yield return new WaitForSecondsRealtime(0.1f);
			timeRemaining -= 0.1f;
		}
		RevertChanges();
	}

	public void ConfirmChanges()
	{
		if (countdownCoroutine != null)
		{
			StopCoroutine(countdownCoroutine);
			countdownCoroutine = null;
		}
		if (newSettings != null)
		{
			newSettings.SaveToDisk();
		}
		if (confirmationPanel != null)
		{
			confirmationPanel.SetActive(value: false);
		}
		this.OnConfirmed?.Invoke();
	}

	public void RevertChanges()
	{
		if (countdownCoroutine != null)
		{
			StopCoroutine(countdownCoroutine);
			countdownCoroutine = null;
		}
		if (previousSettings != null)
		{
			previousSettings.Apply();
		}
		if (confirmationPanel != null)
		{
			confirmationPanel.SetActive(value: false);
		}
		this.OnReverted?.Invoke();
	}

	public void Hide()
	{
		if (countdownCoroutine != null)
		{
			StopCoroutine(countdownCoroutine);
			countdownCoroutine = null;
		}
		if (confirmationPanel != null)
		{
			confirmationPanel.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (countdownCoroutine != null)
		{
			StopCoroutine(countdownCoroutine);
		}
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
