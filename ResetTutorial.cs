using UnityEngine;

public class ResetTutorial : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void TutorialReset()
	{
		if (SaveManager.Instance != null)
		{
			SaveManager.Instance.ResetTutorialState();
		}
		else
		{
			Debug.LogError("SaveManager.Instance is null. Cannot reset tutorial state.");
		}
	}
}
