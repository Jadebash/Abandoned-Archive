using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SpellSlot
{
	public string slotName;

	[SerializeField]
	private bool _isUsable;

	[SerializeField]
	private bool _isActive = true;

	public Spell spell;

	public GameObject UIObject;

	public Image UIBackground;

	public Slider slider;

	public Image iconUI;

	public Animator animator;

	[HideInInspector]
	public float cooldownTimer;

	private bool _isVisuallyCharged;

	private bool _isVisuallyActive;

	private List<CooldownPenalty> cooldownPenalties = new List<CooldownPenalty>();

	public bool isUsable => _isUsable;

	public bool isActive => _isActive;

	public virtual void SetNewSpell(Spell newSpell)
	{
		if (newSpell == null)
		{
			return;
		}
		if (!isUsable)
		{
			Debug.LogWarning("Assigned spell to unusable spell slot");
		}
		spell = newSpell;
		spell.comboCounter = 0;
		cooldownTimer = 0f;
		if (isActive)
		{
			if (slider != null)
			{
				slider.maxValue = spell.cooldownTime;
			}
			if (iconUI != null)
			{
				iconUI.sprite = spell.icon;
			}
			if (iconUI != null)
			{
				iconUI.gameObject.SetActive(value: true);
			}
		}
	}

	public void Enable()
	{
		_isUsable = true;
		UIObject.SetActive(value: true);
	}

	public void Disable()
	{
		_isUsable = false;
		UIObject.SetActive(value: false);
	}

	public void SetActive(bool active)
	{
		_isActive = active;
		if (UIObject != null)
		{
			UIObject.SetActive(active);
		}
		if (active && spell != null)
		{
			if (slider != null)
			{
				slider.maxValue = spell.cooldownTime;
			}
			if (iconUI != null)
			{
				iconUI.sprite = spell.icon;
				iconUI.gameObject.SetActive(value: true);
			}
			_isVisuallyActive = true;
		}
		else
		{
			if (iconUI != null)
			{
				iconUI.sprite = null;
				iconUI.gameObject.SetActive(value: false);
			}
			ResetVisualState(UIBackground.color);
			_isVisuallyActive = false;
		}
	}

	public void UpdateUI(Color stanceColor)
	{
		if (!_isActive || spell == null)
		{
			if (_isActive && spell == null && animator != null)
			{
				ResetVisualState(stanceColor);
			}
			return;
		}
		if (slider != null)
		{
			slider.value = Mathf.Clamp(cooldownTimer, 0f, slider.maxValue);
			slider.maxValue = spell.cooldownTime;
		}
		if (UIBackground != null)
		{
			UIBackground.color = stanceColor;
		}
		bool flag = cooldownTimer >= spell.cooldownTime;
		if (flag != _isVisuallyCharged)
		{
			_isVisuallyCharged = flag;
			if (animator != null)
			{
				animator.SetBool("Charged", _isVisuallyCharged);
			}
		}
	}

	public void ResetVisualState(Color stanceColor)
	{
		slider.value = 0f;
		if (animator != null)
		{
			animator.SetBool("Charged", value: false);
		}
		if (UIBackground != null)
		{
			UIBackground.color = stanceColor;
		}
		_isVisuallyCharged = false;
	}

	public void TriggerPressAnimation()
	{
		if (animator != null && _isActive)
		{
			animator.SetTrigger("Press");
		}
	}

	public void AddCooldownPenalty(float penaltyAmount, float duration)
	{
		cooldownPenalties.Add(new CooldownPenalty(Time.time + duration, penaltyAmount));
	}

	public void CleanupExpiredPenalties()
	{
		cooldownPenalties.RemoveAll((CooldownPenalty penalty) => Time.time >= penalty.expiryTime);
	}

	public float GetCooldownSpeedModifier()
	{
		float num = 0f;
		foreach (CooldownPenalty cooldownPenalty in cooldownPenalties)
		{
			if (Time.time < cooldownPenalty.expiryTime)
			{
				num += cooldownPenalty.penaltyAmount;
			}
		}
		return Mathf.Max(0.1f, 1f - num);
	}
}
