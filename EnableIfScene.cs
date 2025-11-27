using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableIfScene : MonoBehaviour
{
	public string sceneName;

	private void OnEnable()
	{
		if (SceneManager.GetActiveScene().name != sceneName)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
