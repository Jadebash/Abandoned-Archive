using UnityEngine;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
	public static Settings Instance;

	private bool inSettingsMenu;

	private void Awake()
	{
		SceneManager.sceneUnloaded += UnloadedScene;
		Instance = this;
	}

	public void PressMenu(bool sound = false, GameObject activator = null)
	{
		if (!inSettingsMenu && Time.timeScale == 1f && !GameObject.Find("SpellSelection") && Time.timeScale != 0f && (!(Tutorial.Instance != null) || !Tutorial.Instance.inIntroAnimation))
		{
			Manager.Instance.PauseGame(pauseMusic: false, activator);
			bool flag = SceneManager.GetActiveScene().name == "Menu";
			inSettingsMenu = true;
			if (MusicManager.Instance != null && !flag)
			{
				MusicManager.EnterMenu();
			}
			SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
		}
	}

	private void UnloadedScene(Scene scene)
	{
		if (scene.name == "Settings")
		{
			inSettingsMenu = false;
			bool flag = SceneManager.GetActiveScene().name == "Menu";
			if (MusicManager.Instance != null && !flag)
			{
				MusicManager.ExitMenu();
			}
			Manager.Instance.UnpauseGame();
		}
	}

	private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= UnloadedScene;
	}
}
