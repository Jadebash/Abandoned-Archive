using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalisedIntroUI : MonoBehaviour
{
	public LocalisedString[] floorNames;

	private void Start()
	{
		Debug.Log(SceneManager.GetActiveScene().name);
		if (SceneManager.GetActiveScene().buildIndex == 4)
		{
			GetComponent<TMP_Text>().text = floorNames[0].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 5)
		{
			GetComponent<TMP_Text>().text = floorNames[1].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 6)
		{
			GetComponent<TMP_Text>().text = floorNames[2].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 7)
		{
			GetComponent<TMP_Text>().text = floorNames[3].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 3)
		{
			GetComponent<TMP_Text>().text = floorNames[4].value;
		}
	}

	private void OnEnable()
	{
		Debug.Log(SceneManager.GetActiveScene().buildIndex);
		if (SceneManager.GetActiveScene().buildIndex == 4)
		{
			GetComponent<TMP_Text>().text = floorNames[0].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 5)
		{
			GetComponent<TMP_Text>().text = floorNames[1].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 6)
		{
			GetComponent<TMP_Text>().text = floorNames[2].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 7)
		{
			GetComponent<TMP_Text>().text = floorNames[3].value;
		}
		else if (SceneManager.GetActiveScene().buildIndex == 3)
		{
			GetComponent<TMP_Text>().text = floorNames[4].value;
		}
	}
}
