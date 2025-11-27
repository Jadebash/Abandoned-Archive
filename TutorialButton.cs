using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour
{
	public void SkipTutorial()
	{
		Tutorial.FinishedTutorial();
		Object.Destroy(Object.FindObjectOfType<Settings>());
		SceneManager.UnloadSceneAsync("Settings");
	}
}
