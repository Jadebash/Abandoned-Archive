using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpellPicker : MonoBehaviour
{
	public TextMeshProUGUI spellName;

	public TextMeshProUGUI spellFlavour;

	public TextMeshProUGUI spellCooldown;

	public TextMeshProUGUI spellClass;

	public Image icon;

	public Image spellClassIcon;

	public TextMeshProUGUI description;

	public TextMeshProUGUI upgradeDescription;

	public TextMeshProUGUI actionDescription;

	public Button button;

	public bool first;

	public void Start()
	{
		if (first)
		{
			button.Select();
		}
	}

	public void OnEnable()
	{
		if (first)
		{
			button.OnSelect(null);
		}
	}
}
