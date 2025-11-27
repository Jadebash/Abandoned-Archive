using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocaliserUI : MonoBehaviour
{
	private TextMeshProUGUI textField;

	private LocalisedString _localisedString;

	public LocalisedString localisedString
	{
		get
		{
			return _localisedString;
		}
		set
		{
			_localisedString = value;
			UpdateText();
		}
	}

	private void Start()
	{
		textField = GetComponent<TextMeshProUGUI>();
		UpdateText();
	}

	private void OnEnable()
	{
		textField = GetComponent<TextMeshProUGUI>();
		UpdateText();
	}

	private void UpdateText()
	{
		if (!string.IsNullOrEmpty(localisedString.key))
		{
			if (textField == null)
			{
				textField = GetComponent<TextMeshProUGUI>();
			}
			if (textField != null)
			{
				textField.text = localisedString.value;
			}
		}
	}
}
