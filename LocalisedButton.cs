using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalisedButton : MonoBehaviour
{
	public LocalisedString ButtonText;

	private void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex == 1 && GetComponent<TMP_Text>().text != ButtonText.value)
		{
			GetComponent<TMP_Text>().text = ButtonText.value;
		}
	}

	private void Start()
	{
		GetComponent<TMP_Text>().text = ButtonText.value;
	}

	private void OnEnable()
	{
		GetComponent<TMP_Text>().text = ButtonText.value;
	}
}
