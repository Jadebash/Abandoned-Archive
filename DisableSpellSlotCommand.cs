using UnityEngine;

[CreateAssetMenu(fileName = "Disable Spell Slot Command", menuName = "Abandoned Archive/Debug/Commands/Disable Spell Slot Command")]
public class DisableSpellSlotCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (args.Length != 1)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: disableslot <slot_number>");
		}
		if (!int.TryParse(args[0], out var result))
		{
			return Fail(CommandFailType.Custom, "'" + args[0] + "' is not a valid slot number. Please provide an integer.");
		}
		if (SpellCasting.Instances == null || SpellCasting.Instances.Count == 0 || SpellCasting.Instances[0] == null)
		{
			return Fail(CommandFailType.Custom, "SpellCasting instance not found.");
		}
		if (result < 0 || result >= SpellCasting.Instances[0].currentStance.spellSlots.Count)
		{
			return Fail(CommandFailType.Custom, $"Slot {result} is out of range. Valid slots: 0-{SpellCasting.Instances[0].currentStance.spellSlots.Count - 1}");
		}
		SpellCasting.Instances[0].currentStance.spellSlots[result].Disable();
		return Success("Disabled slot " + result + ".");
	}
}
