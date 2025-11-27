using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
	public AudioSource audioSource;

	public float beginToFadeTimer = 1f;

	public float audioFade;

	public Animator fadeAnim;

	private bool isFading;

	public void Start()
	{
		isFading = false;
	}

	public void Restart()
	{
		isFading = true;
		if (beginToFadeTimer <= 0f)
		{
			SceneManager.LoadScene("Intro");
		}
	}

	public void Quit()
	{
		Debug.Log("Application quit");
		Application.Quit();
	}

	public void StartGame()
	{
		SceneManager.LoadScene("Intro");
	}

	private void Update()
	{
		if (isFading)
		{
			beginToFadeTimer -= Time.deltaTime;
			fadeAnim.SetBool("isFading", value: true);
			if (beginToFadeTimer <= 0f)
			{
				SceneManager.LoadScene("Intro");
			}
		}
	}
}
