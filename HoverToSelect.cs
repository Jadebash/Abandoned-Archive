using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoverToSelect : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	private Button button;

	private List<HoverToSelect> others = new List<HoverToSelect>();

	private void Start()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(Click);
		HoverToSelect[] array = Object.FindObjectsOfType<HoverToSelect>();
		foreach (HoverToSelect hoverToSelect in array)
		{
			if (hoverToSelect != this)
			{
				others.Add(hoverToSelect);
			}
		}
	}

	public void Hover()
	{
		if (button.interactable && Application.isFocused)
		{
			button.Select();
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		RuntimeManager.PlayOneShot("event:/SFX/UI/Hover", base.transform.position);
	}

	public void Click()
	{
		RuntimeManager.PlayOneShot("event:/SFX/UI/Click", base.transform.position);
	}
}
