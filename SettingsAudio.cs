using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsAudio : MonoBehaviour
{
	private void OnEnable()
	{
		bool flag = SceneManager.GetActiveScene().name == "Menu";
		if (MusicManager.Instance != null && !flag)
		{
			MusicManager.ExitMenu();
		}
	}

	private void OnDisable()
	{
		bool flag = SceneManager.GetActiveScene().name == "Menu";
		if (MusicManager.Instance != null && !flag)
		{
			MusicManager.EnterMenu();
		}
	}
}
