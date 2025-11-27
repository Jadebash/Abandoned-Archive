using UnityEngine;
using UnityEngine.UI;

public class UIScreenshake : MonoBehaviour
{
	public void SetScreenshakeIntensity(float intensity)
	{
		if (SaveManager.Instance != null)
		{
			Save currentSave = SaveManager.Instance.currentSave;
			currentSave.screenshakeIntensity = intensity / 10f;
			SaveManager.Instance.currentSave = currentSave;
		}
	}

	private void OnEnable()
	{
		Slider component = GetComponent<Slider>();
		if (component != null)
		{
			component.value = SaveManager.Instance.currentSave.screenshakeIntensity * 10f;
		}
	}
}
