using UnityEngine;

public class BetaEnding : MonoBehaviour
{
	public GameObject endingUI;

	private void Start()
	{
		if (endingUI != null)
		{
			endingUI.SetActive(value: false);
		}
	}

	public void ShowEnding()
	{
		if (endingUI != null)
		{
			endingUI.SetActive(value: true);
			if (MusicManager.Instance != null)
			{
				MusicManager.Instance.FadeOutAllMusic();
				MusicManager.Calm();
			}
			if (Manager.Instance != null)
			{
				Manager.Instance.PauseGame();
			}
		}
	}
}
