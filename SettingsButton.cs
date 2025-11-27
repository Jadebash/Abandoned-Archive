using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
	private bool inSettingsMenu;

	private void Awake()
	{
		SceneManager.sceneUnloaded += UnloadedScene;
	}

	public void Settings()
	{
		if (!inSettingsMenu)
		{
			inSettingsMenu = true;
			SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
		}
		else
		{
			SceneManager.UnloadSceneAsync("Settings");
		}
	}

	private void UnloadedScene(Scene scene)
	{
		if (scene.name == "Settings")
		{
			inSettingsMenu = false;
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= UnloadedScene;
		if (inSettingsMenu)
		{
			SceneManager.UnloadSceneAsync("Settings");
		}
	}
}
