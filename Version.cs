using TMPro;
using UnityEngine;

public class Version : MonoBehaviour
{
	public static Version Instance;

	public string phase;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(phase))
		{
			GetComponent<TextMeshProUGUI>().text = "Abandoned Archive v" + Application.version;
		}
		else
		{
			GetComponent<TextMeshProUGUI>().text = "Abandoned Archive [" + phase + "] v" + Application.version;
		}
	}
}
