using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
	public bool loadPlayerFirst = true;

	public bool loadManagerWithoutPlayer;

	private void Awake()
	{
		if (Manager.Instance == null)
		{
			if (loadPlayerFirst)
			{
				SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
				SceneManager.sceneLoaded += LoadedScene;
			}
			else if (loadManagerWithoutPlayer)
			{
				SceneManager.LoadSceneAsync("Manager", LoadSceneMode.Additive);
				SceneManager.sceneLoaded += LoadedScene;
			}
		}
	}

	private void LoadedScene(Scene scene, LoadSceneMode sceneLoadMode)
	{
		if (scene.name == "Player")
		{
			SceneManager.LoadSceneAsync("Manager", LoadSceneMode.Additive);
		}
		else if (scene.name == "Manager")
		{
			SceneManager.sceneLoaded -= LoadedScene;
			Difficulty component = GetComponent<Difficulty>();
			if (component != null)
			{
				Object.FindObjectOfType<FloorManager>().PlayedThroughEditor(SceneManager.GetActiveScene().name, component.floor + 1);
			}
			else
			{
				Object.FindObjectOfType<FloorManager>().PlayedThroughEditor(SceneManager.GetActiveScene().name);
			}
		}
	}
}
