using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAdditive : MonoBehaviour
{
	public string sceneName = "";

	public bool onStart = true;

	public bool resetPlayer = true;

	public bool moveChildren = true;

	public bool doUnload = true;

	private Scene cachedActiveScene;

	private Dictionary<GameObject, bool> oldScene = new Dictionary<GameObject, bool>();

	private Vector3 previousPosition;

	private void Start()
	{
		SceneManager.sceneLoaded += SceneLoaded;
		if (doUnload)
		{
			SceneManager.sceneUnloaded += SceneUnloaded;
		}
		if (onStart)
		{
			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		}
	}

	private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (!(scene.name == sceneName))
		{
			return;
		}
		GameObject[] players;
		if (resetPlayer)
		{
			players = PlayerManager.players;
			foreach (GameObject gameObject in players)
			{
				previousPosition = gameObject.transform.position;
				Movement component = gameObject.GetComponent<Movement>();
				if (component != null)
				{
					component.SafeTeleport(Vector3.zero);
				}
			}
			CameraController.Instance.transform.position = Vector3.zero;
		}
		if (moveChildren && base.gameObject != null)
		{
			foreach (Transform item in base.transform)
			{
				item.parent = null;
				SceneManager.MoveGameObjectToScene(item.gameObject, scene);
			}
		}
		oldScene = new Dictionary<GameObject, bool>();
		cachedActiveScene = SceneManager.GetActiveScene();
		players = cachedActiveScene.GetRootGameObjects();
		foreach (GameObject gameObject2 in players)
		{
			oldScene.Add(gameObject2, gameObject2.activeSelf);
			gameObject2.SetActive(value: false);
		}
		SceneManager.SetActiveScene(scene);
	}

	private void SceneUnloaded(Scene scene)
	{
		if (!(scene.name == sceneName))
		{
			return;
		}
		if (cachedActiveScene.IsValid())
		{
			SceneManager.SetActiveScene(cachedActiveScene);
		}
		foreach (GameObject key in oldScene.Keys)
		{
			if (key != null)
			{
				key.SetActive(oldScene[key]);
			}
		}
		if (!resetPlayer)
		{
			return;
		}
		GameObject[] players = PlayerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			Movement component = players[i].GetComponent<Movement>();
			if (component != null)
			{
				component.SafeTeleport(previousPosition);
			}
		}
		CameraController.Instance.transform.position = previousPosition;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= SceneLoaded;
		if (doUnload)
		{
			SceneManager.sceneUnloaded -= SceneUnloaded;
		}
	}
}
