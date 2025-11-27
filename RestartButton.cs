using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
	public void Restart()
	{
		if (Manager.Instance != null && Manager.Instance.isPaused)
		{
			Manager.Instance.UnpauseGame();
		}
		if (SceneManager.GetSceneByName("Settings").IsValid())
		{
			Settings settings = Object.FindObjectOfType<Settings>();
			if (settings != null)
			{
				Object.Destroy(settings.gameObject);
			}
			SceneManager.UnloadSceneAsync("Settings");
		}
		if (Manager.Instance != null)
		{
			Manager.Instance.Restart();
		}
		else
		{
			Debug.LogError("Manager.Instance is null! Cannot restart game.");
		}
	}
}
