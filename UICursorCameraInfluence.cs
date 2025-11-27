using UnityEngine;
using UnityEngine.UI;

public class UICursorCameraInfluence : MonoBehaviour
{
	public void SetCursorCameraInfluence(float influence)
	{
		if (SaveManager.Instance != null)
		{
			Save currentSave = SaveManager.Instance.currentSave;
			currentSave.cursorCameraInfluence = influence / 10f;
			SaveManager.Instance.currentSave = currentSave;
		}
	}

	private void OnEnable()
	{
		Slider component = GetComponent<Slider>();
		if (component != null)
		{
			component.value = SaveManager.Instance.currentSave.cursorCameraInfluence * 10f;
		}
	}
}
