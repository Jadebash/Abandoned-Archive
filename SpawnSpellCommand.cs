using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Spell Command", menuName = "Abandoned Archive/Debug/Commands/Spawn Spell Command")]
public class SpawnSpellCommand : ConsoleCommand
{
	public List<Spell> additionalSpells = new List<Spell>();

	public override CommandResponse Process(string[] args)
	{
		if (args.Length != 1)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: spawnspell <spell_id> or spawnspell --list");
		}
		if (args[0].Equals("--list", StringComparison.OrdinalIgnoreCase))
		{
			if (RunManager.Instance == null)
			{
				return Fail(CommandFailType.Custom, "Cannot list spells: Not in a run.");
			}
			string text = "<b>Available Spells:</b>\n";
			text += "────────────────────────────────────────\n";
			List<Spell> list = new List<Spell>();
			FieldInfo field = typeof(RunManager).GetField("allSpells", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				list = (List<Spell>)field.GetValue(RunManager.Instance);
			}
			list.AddRange(additionalSpells);
			if (list.Count == 0)
			{
				return Fail(CommandFailType.Custom, "No spells available in RunManager.");
			}
			foreach (Spell item in list)
			{
				string text2 = item.GetType().ToString();
				string text3 = ((!string.IsNullOrEmpty(item.name.value)) ? item.name.value : "Unknown");
				text = text + "<color=#00DDFF>" + text2 + "</color>\n  <color=#AAAAAA>" + text3 + "</color>\n\n";
			}
			text += "────────────────────────────────────────\n";
			text += $"<i>Total: {list.Count} spells</i>";
			return Success(text);
		}
		Spell spell = additionalSpells.Find((Spell spell2) => spell2.GetType().ToString() == args[0]);
		if (spell != null)
		{
			spell = RunManager.Instance.SpawnSpell(spell);
			return Success("Spawned " + spell.name.value + ".");
		}
		if (RunManager.Instance == null)
		{
			return Fail(CommandFailType.Custom, "Cannot spawn that spell: Not in a run.");
		}
		spell = RunManager.Instance.SpawnSpell(args[0]);
		if (spell == null)
		{
			return Fail(CommandFailType.Custom, "Spell '" + args[0] + "' not found. Use 'spawnspell --list' to see available spells.");
		}
		return Success("Spawned " + spell.name.value + ".");
	}
}
