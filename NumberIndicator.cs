using TMPro;
using UnityEngine;

public class NumberIndicator : MonoBehaviour
{
	public TextMeshProUGUI numberText;

	public string number;

	private void Start()
	{
		if (float.TryParse(number, out var result))
		{
			result = Mathf.Round(result);
			numberText.text = result.ToString();
		}
		else
		{
			numberText.text = number;
		}
	}
}
