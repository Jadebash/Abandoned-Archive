using UnityEngine;

public class QuitButton : MonoBehaviour
{
	public GameObject confirmQuitModal;

	public void Quit()
	{
		if (confirmQuitModal != null)
		{
			confirmQuitModal.SetActive(value: true);
		}
		else
		{
			Application.Quit();
		}
	}
}
