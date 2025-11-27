using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class SetScreenSpaceCamera : MonoBehaviour
{
	private Canvas canvas;

	private void Start()
	{
		canvas = GetComponent<Canvas>();
		canvas.sortingLayerID = SortingLayer.GetLayerValueFromName("UI");
		canvas.sortingLayerName = "UI";
		if (!TryAssigningCamera())
		{
			SceneManager.sceneLoaded += LoadedScene;
		}
	}

	private void LoadedScene(Scene scene, LoadSceneMode loadSceneMode)
	{
		TryAssigningCamera();
	}

	private bool TryAssigningCamera()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			canvas.worldCamera = main;
			canvas.sortingLayerID = SortingLayer.GetLayerValueFromName("UI");
			canvas.sortingLayerName = "UI";
			return true;
		}
		return false;
	}
}
