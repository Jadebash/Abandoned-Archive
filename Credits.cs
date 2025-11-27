using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
	public void StopCreditsAction(InputAction.CallbackContext context)
	{
		if (context.canceled)
		{
			StopCredits();
		}
	}

	public void StopCredits()
	{
		if (Ending.Instance != null)
		{
			Ending.Instance.StartCoroutine(StopCreditsCoroutine());
		}
	}

	private IEnumerator StopCreditsCoroutine()
	{
		if (Ending.Instance != null)
		{
			Ending.Instance.StopEndingMusic();
		}
		yield return SceneManager.UnloadSceneAsync("Credits");
		if (PlayerManager.Instance != null)
		{
			PlayerManager.Instance.ClearPlayers();
		}
		yield return SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
		Scene sceneByName = SceneManager.GetSceneByName("Menu");
		if (sceneByName.IsValid())
		{
			SceneManager.SetActiveScene(sceneByName);
		}
	}
}
