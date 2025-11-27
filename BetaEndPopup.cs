using UnityEngine;
using UnityEngine.SceneManagement;

public class BetaEndPopup : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void LoadMenu()
	{
		if (Manager.Instance != null)
		{
			Manager.Instance.UnpauseGame(unpauseMusic: true);
		}
		SceneManager.LoadScene("Menu");
	}
}
