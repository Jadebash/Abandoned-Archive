using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayer : MonoBehaviour
{
	private void Awake()
	{
		if (Manager.Instance == null)
		{
			SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
		}
	}
}
