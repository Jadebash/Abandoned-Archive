public class SpellPickup : Interactable
{
	public Spell spell;

	public bool randomSpell;

	private void Start()
	{
		if (randomSpell)
		{
			spell = RunManager.GetSpell();
		}
		else if (spriteIcon != null && spell != null)
		{
			spriteIcon.sprite = spell.icon;
		}
	}
}
