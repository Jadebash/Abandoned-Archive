using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableIfScene : MonoBehaviour
{
	public string sceneName;

	private void OnEnable()
	{
		if (SceneManager.GetActiveScene().name == sceneName)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
