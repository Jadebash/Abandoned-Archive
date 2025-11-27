using UnityEngine;
using UnityEngine.UI;

public class UIScaling : MonoBehaviour
{
	public delegate void UIScaleChanged(float scale);

	public static event UIScaleChanged OnUIScaleChanged;

	public void SetSliderScale(float scale)
	{
		SetUIScale(scale / 10f);
	}

	public static void BroadcastUIScaleChanged()
	{
		UIScaling.OnUIScaleChanged?.Invoke(GetUIScale());
	}

	public static void SetUIScale(float scale)
	{
		UIScaling.OnUIScaleChanged?.Invoke(scale);
		if (SaveManager.Instance != null)
		{
			Save currentSave = SaveManager.Instance.currentSave;
			currentSave.uiScale = scale;
			SaveManager.Instance.currentSave = currentSave;
		}
	}

	public static float GetUIScale()
	{
		if (SaveManager.Instance == null)
		{
			return 1f;
		}
		if (SaveManager.Instance.currentSave == null)
		{
			return 1f;
		}
		return SaveManager.Instance.currentSave.uiScale;
	}

	private void OnEnable()
	{
		Slider component = GetComponent<Slider>();
		if (component != null)
		{
			component.value = GetUIScale() * 10f;
		}
	}
}
