using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EffectsVisualiser : MonoBehaviour
{
	public enum EffectType
	{
		Speed = 0,
		Roll = 1,
		Damage = 2,
		IncomingDamage = 3,
		Knowledge = 4,
		Cooldown = 5
	}

	public static EffectsVisualiser Instance;

	public GameObject tooltipPanel;

	public TextMeshProUGUI tooltipText;

	public Canvas canvas;

	private EffectVisual currentHoveredEffect;

	public List<EffectVisual> effects = new List<EffectVisual>(Enum.GetValues(typeof(EffectType)).Length);

	private void Awake()
	{
		Instance = this;
		foreach (EffectVisual effect in effects)
		{
			if (effect.increase != null)
			{
				effect.increaseImage = effect.increase.transform.GetChild(0).GetComponent<Image>();
				AddHoverListener(effect.increase.gameObject, effect);
			}
			if (effect.decrease != null)
			{
				effect.decreaseImage = effect.decrease.transform.GetChild(0).GetComponent<Image>();
				AddHoverListener(effect.decrease.gameObject, effect);
			}
		}
		if (tooltipPanel != null)
		{
			tooltipPanel.SetActive(value: false);
		}
	}

	private void AddHoverListener(GameObject obj, EffectVisual effect)
	{
		EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = obj.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener(delegate
		{
			OnHoverEnter(effect);
		});
		eventTrigger.triggers.Add(entry);
		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerExit;
		entry2.callback.AddListener(delegate
		{
			OnHoverExit();
		});
		eventTrigger.triggers.Add(entry2);
	}

	private void OnHoverEnter(EffectVisual effect)
	{
		currentHoveredEffect = effect;
		if (tooltipPanel != null && effect.currentValue != 1f)
		{
			tooltipPanel.SetActive(value: true);
			if (tooltipText != null)
			{
				tooltipText.text = effect.currentValue.ToString("F2") + "x";
			}
		}
	}

	private void OnHoverExit()
	{
		currentHoveredEffect = null;
		if (tooltipPanel != null)
		{
			tooltipPanel.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (tooltipPanel != null && tooltipPanel.activeSelf && canvas != null)
		{
			Vector3 mousePosition = Input.mousePosition;
			Vector3 position = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, canvas.planeDistance));
			tooltipPanel.transform.position = position;
			if (currentHoveredEffect != null && tooltipText != null)
			{
				tooltipText.text = currentHoveredEffect.currentValue.ToString("F2") + "x";
			}
		}
	}

	public void SetEffectVisual(EffectType type, float value)
	{
		effects[(int)type].currentValue = value;
		if (value > 1.01f)
		{
			if (effects[(int)type].increase != null)
			{
				effects[(int)type].increase.AnimateIn();
				Color color = effects[(int)type].increaseImage.color;
				color.a = Mathf.Clamp01(value / 2f);
				effects[(int)type].increaseImage.color = color;
			}
			if (effects[(int)type].decrease != null && effects[(int)type].decrease.gameObject.activeSelf)
			{
				effects[(int)type].decrease.AnimateOut();
			}
		}
		else if (value < 0.99f)
		{
			if (effects[(int)type].decrease != null)
			{
				effects[(int)type].decrease.AnimateIn();
				Color color2 = effects[(int)type].decreaseImage.color;
				color2.a = Mathf.Clamp01(1f - value / 2f);
				effects[(int)type].decreaseImage.color = color2;
			}
			if (effects[(int)type].increase != null && effects[(int)type].increase.gameObject.activeSelf)
			{
				effects[(int)type].increase.AnimateOut();
			}
		}
		else
		{
			if (effects[(int)type].decrease != null && effects[(int)type].decrease.gameObject.activeSelf)
			{
				effects[(int)type].decrease.AnimateOut();
			}
			if (effects[(int)type].increase != null && effects[(int)type].increase.gameObject.activeSelf)
			{
				effects[(int)type].increase.AnimateOut();
			}
		}
	}
}
