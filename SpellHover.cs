using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellHover : MonoBehaviour
{
	public GameObject spellTooltip;

	private string spellNameText;

	private string spellDescText;

	public SpellCasting owner;

	public void StartHoverSpecial()
	{
		if (spellTooltip == null || (WaveManager.Instance != null && WaveManager.Instance.playerInRoom) || !(owner.currentStance.spellSlots[0].spell != null))
		{
			return;
		}
		Animator component = spellTooltip.GetComponent<Animator>();
		if (component != null && component.runtimeAnimatorController != null)
		{
			component.SetBool("FadeOut", value: false);
		}
		spellNameText = owner.currentStance.spellSlots[0].spell.name.value;
		if (owner.currentStance.spellSlots[0].spell.upgradeCount > 0)
		{
			spellNameText = spellNameText + " +" + owner.currentStance.spellSlots[0].spell.upgradeCount;
		}
		spellDescText = owner.currentStance.spellSlots[0].spell.description.value;
		spellTooltip.transform.Find("SpellNameText").gameObject.GetComponent<TextMeshProUGUI>().text = spellNameText;
		spellTooltip.transform.Find("SpellDescText").gameObject.GetComponent<TextMeshProUGUI>().text = spellDescText;
		spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: false);
		spellTooltip.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		foreach (Transform item in spellTooltip.transform)
		{
			if (item.GetComponent<TextMeshProUGUI>() != null)
			{
				TextMeshProUGUI component2 = item.GetComponent<TextMeshProUGUI>();
				component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, 1f);
			}
		}
		spellTooltip.SetActive(value: true);
	}

	public void StartHoverPrimary()
	{
		if (spellTooltip == null || (WaveManager.Instance != null && WaveManager.Instance.playerInRoom) || !(owner.currentStance.spellSlots[1].spell != null))
		{
			return;
		}
		Animator component = spellTooltip.GetComponent<Animator>();
		if (component != null && component.runtimeAnimatorController != null)
		{
			component.SetBool("FadeOut", value: false);
		}
		spellNameText = owner.currentStance.spellSlots[1].spell.name.value;
		if (owner.currentStance.spellSlots[1].spell.upgradeCount > 0)
		{
			spellNameText = spellNameText + " +" + owner.currentStance.spellSlots[1].spell.upgradeCount;
		}
		spellDescText = owner.currentStance.spellSlots[1].spell.description.value;
		spellTooltip.transform.Find("SpellNameText").gameObject.GetComponent<TextMeshProUGUI>().text = spellNameText;
		spellTooltip.transform.Find("SpellDescText").gameObject.GetComponent<TextMeshProUGUI>().text = spellDescText;
		if (owner.currentStance.spellSlots[1].spell.charge)
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: true);
		}
		else
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: false);
		}
		spellTooltip.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		foreach (Transform item in spellTooltip.transform)
		{
			if (item.GetComponent<TextMeshProUGUI>() != null)
			{
				TextMeshProUGUI component2 = item.GetComponent<TextMeshProUGUI>();
				component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, 1f);
			}
		}
		spellTooltip.SetActive(value: true);
	}

	public void EndHover()
	{
		if (!(spellTooltip == null))
		{
			Animator component = spellTooltip.GetComponent<Animator>();
			if (base.gameObject.activeInHierarchy && component != null && component.runtimeAnimatorController != null)
			{
				component.SetBool("FadeOut", value: true);
			}
		}
	}

	public void StartHoverSecondary()
	{
		if (spellTooltip == null || (WaveManager.Instance != null && WaveManager.Instance.playerInRoom) || !(owner.currentStance.spellSlots[2].spell != null))
		{
			return;
		}
		Animator component = spellTooltip.GetComponent<Animator>();
		if (component != null && component.runtimeAnimatorController != null)
		{
			component.SetBool("FadeOut", value: false);
		}
		spellNameText = owner.currentStance.spellSlots[2].spell.name.value;
		if (owner.currentStance.spellSlots[2].spell.upgradeCount > 0)
		{
			spellNameText = spellNameText + " +" + owner.currentStance.spellSlots[2].spell.upgradeCount;
		}
		spellDescText = owner.currentStance.spellSlots[2].spell.description.value;
		spellTooltip.transform.Find("SpellNameText").gameObject.GetComponent<TextMeshProUGUI>().text = spellNameText;
		spellTooltip.transform.Find("SpellDescText").gameObject.GetComponent<TextMeshProUGUI>().text = spellDescText;
		if (owner.currentStance.spellSlots[2].spell.charge)
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: true);
		}
		else
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: false);
		}
		spellTooltip.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		foreach (Transform item in spellTooltip.transform)
		{
			if (item.GetComponent<TextMeshProUGUI>() != null)
			{
				TextMeshProUGUI component2 = item.GetComponent<TextMeshProUGUI>();
				component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, 1f);
			}
		}
		spellTooltip.SetActive(value: true);
	}

	public void StartHoverExtraSlot1()
	{
		if (spellTooltip == null || (WaveManager.Instance != null && WaveManager.Instance.playerInRoom))
		{
			return;
		}
		Stance stance = null;
		foreach (Stance stance2 in owner.stances)
		{
			if (stance2 != owner.currentStance && stance2.enabled)
			{
				stance = stance2;
				break;
			}
		}
		if (stance == null || stance.spellSlots.Count < 2 || !(stance.spellSlots[1].spell != null))
		{
			return;
		}
		Animator component = spellTooltip.GetComponent<Animator>();
		if (component != null && component.runtimeAnimatorController != null)
		{
			component.SetBool("FadeOut", value: false);
		}
		spellNameText = stance.spellSlots[1].spell.name.value;
		if (stance.spellSlots[1].spell.upgradeCount > 0)
		{
			spellNameText = spellNameText + " +" + stance.spellSlots[1].spell.upgradeCount;
		}
		spellDescText = stance.spellSlots[1].spell.description.value;
		spellTooltip.transform.Find("SpellNameText").gameObject.GetComponent<TextMeshProUGUI>().text = spellNameText;
		spellTooltip.transform.Find("SpellDescText").gameObject.GetComponent<TextMeshProUGUI>().text = spellDescText;
		if (stance.spellSlots[1].spell.charge)
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: true);
		}
		else
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: false);
		}
		spellTooltip.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		foreach (Transform item in spellTooltip.transform)
		{
			if (item.GetComponent<TextMeshProUGUI>() != null)
			{
				TextMeshProUGUI component2 = item.GetComponent<TextMeshProUGUI>();
				component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, 1f);
			}
		}
		spellTooltip.SetActive(value: true);
	}

	public void StartHoverExtraSlot2()
	{
		if (spellTooltip == null || (WaveManager.Instance != null && WaveManager.Instance.playerInRoom))
		{
			return;
		}
		Stance stance = null;
		foreach (Stance stance2 in owner.stances)
		{
			if (stance2 != owner.currentStance && stance2.enabled)
			{
				stance = stance2;
				break;
			}
		}
		if (stance == null || stance.spellSlots.Count < 3 || !(stance.spellSlots[2].spell != null))
		{
			return;
		}
		Animator component = spellTooltip.GetComponent<Animator>();
		if (component != null && component.runtimeAnimatorController != null)
		{
			component.SetBool("FadeOut", value: false);
		}
		spellNameText = stance.spellSlots[2].spell.name.value;
		if (stance.spellSlots[2].spell.upgradeCount > 0)
		{
			spellNameText = spellNameText + " +" + stance.spellSlots[2].spell.upgradeCount;
		}
		spellDescText = stance.spellSlots[2].spell.description.value;
		spellTooltip.transform.Find("SpellNameText").gameObject.GetComponent<TextMeshProUGUI>().text = spellNameText;
		spellTooltip.transform.Find("SpellDescText").gameObject.GetComponent<TextMeshProUGUI>().text = spellDescText;
		if (stance.spellSlots[2].spell.charge)
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: true);
		}
		else
		{
			spellTooltip.transform.Find("ChargeableText").gameObject.SetActive(value: false);
		}
		spellTooltip.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		foreach (Transform item in spellTooltip.transform)
		{
			if (item.GetComponent<TextMeshProUGUI>() != null)
			{
				TextMeshProUGUI component2 = item.GetComponent<TextMeshProUGUI>();
				component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, 1f);
			}
		}
		spellTooltip.SetActive(value: true);
	}

	public void RefreshTooltip()
	{
		if (spellTooltip != null)
		{
			Animator component = spellTooltip.GetComponent<Animator>();
			if (base.gameObject.activeInHierarchy && component != null && component.runtimeAnimatorController != null)
			{
				component.SetBool("FadeOut", value: true);
			}
			spellTooltip.SetActive(value: false);
		}
	}
}
