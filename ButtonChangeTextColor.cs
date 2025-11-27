using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonChangeTextColor : MonoBehaviour
{
	public Color normalColour;

	public Color colourIfDisabled;

	private void Start()
	{
		if (!GetComponent<Button>().interactable)
		{
			base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = colourIfDisabled;
		}
		else
		{
			base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = normalColour;
		}
	}

	private void OnEnable()
	{
		if (!GetComponent<Button>().interactable)
		{
			base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = colourIfDisabled;
		}
		else
		{
			base.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = normalColour;
		}
	}
}
