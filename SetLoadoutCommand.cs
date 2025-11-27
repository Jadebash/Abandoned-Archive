using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Set Loadout Command", menuName = "Abandoned Archive/Debug/Commands/Set Loadout Command")]
public class SetLoadoutCommand : ConsoleCommand
{
	[Serializable]
	public class LoadoutConfig
	{
		public string name;

		public string[] spellIds;

		public string[] relicIds;
	}

	[SerializeField]
	private LoadoutConfig[] predefinedLoadouts = new LoadoutConfig[3]
	{
		new LoadoutConfig
		{
			name = "Boss Fight Loadout",
			spellIds = new string[3] { "AirSpecial", "LightningMain", "LightningMain2" },
			relicIds = new string[2] { "DamageBuffRelic", "HealthPackRelic" }
		},
		new LoadoutConfig
		{
			name = "Speed Run Loadout",
			spellIds = new string[3] { "SunSpecial", "AirMain2", "CrystalMain" },
			relicIds = new string[2] { "FasterRollRelic", "KnowledgeBoostRelic" }
		},
		new LoadoutConfig
		{
			name = "Tank Loadout",
			spellIds = new string[3] { "GravitySpecial", "GravityMain", "WaterMain" },
			relicIds = new string[3] { "ExtraHeartRelic", "DamageBoostRelic", "ReviveRelic" }
		}
	};

	public override CommandResponse Process(string[] args)
	{
		if (args.Length != 1)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: set_loadout <loadout_id> or set_loadout --list");
		}
		if (args[0].Equals("--list", StringComparison.OrdinalIgnoreCase))
		{
			string text = "<b>Available Loadouts:</b>\n";
			text += "────────────────────────────────────────\n";
			for (int i = 0; i < predefinedLoadouts.Length; i++)
			{
				text += $"<color=#00DDFF>{i + 1}. {predefinedLoadouts[i].name}</color>\n";
				text = text + "  <color=#AAAAAA>Spells: " + string.Join(", ", predefinedLoadouts[i].spellIds) + "</color>\n";
				text = text + "  <color=#AAAAAA>Relics: " + string.Join(", ", predefinedLoadouts[i].relicIds) + "</color>\n\n";
			}
			text += "────────────────────────────────────────\n";
			text += $"<i>Total: {predefinedLoadouts.Length} loadouts</i>";
			return Success(text);
		}
		if (!int.TryParse(args[0], out var result))
		{
			return Fail(CommandFailType.Custom, "Invalid loadout ID. Must be a number or '--list'.");
		}
		if (result < 1 || result > predefinedLoadouts.Length)
		{
			return Fail(CommandFailType.Custom, $"Loadout ID must be between 1 and {predefinedLoadouts.Length}. Use 'set_loadout --list' to see available loadouts.");
		}
		if (RunManager.Instance == null)
		{
			return Fail(CommandFailType.Custom, "Cannot set loadout: Not in a run.");
		}
		if (SpellCasting.Instances.Count == 0)
		{
			return Fail(CommandFailType.Custom, "Cannot set loadout: No player found.");
		}
		if (RelicCollector.Instance == null)
		{
			return Fail(CommandFailType.Custom, "Cannot set loadout: RelicCollector not found.");
		}
		LoadoutConfig loadoutConfig = predefinedLoadouts[result - 1];
		ClearCurrentLoadout();
		string text2 = ApplyLoadout(loadoutConfig);
		return Success("Applied loadout '" + loadoutConfig.name + "':\n" + text2);
	}

	private void ClearCurrentLoadout()
	{
		foreach (Relic item in new List<Relic>(RelicCollector.Instance.Relics()))
		{
			RelicCollector.Instance.DropRelic(item);
		}
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			foreach (Stance stance in instance.stances)
			{
				if (!stance.enabled)
				{
					continue;
				}
				foreach (SpellSlot spellSlot in stance.spellSlots)
				{
					if (!(spellSlot.spell != null))
					{
						continue;
					}
					if (typeof(SpecialSpell).IsAssignableFrom(spellSlot.spell.GetType()))
					{
						if (stance == instance.stances[0])
						{
							spellSlot.spell = null;
							if (spellSlot.iconUI != null)
							{
								spellSlot.iconUI.gameObject.SetActive(value: false);
							}
						}
					}
					else
					{
						spellSlot.spell = null;
						if (spellSlot.iconUI != null)
						{
							spellSlot.iconUI.gameObject.SetActive(value: false);
						}
					}
				}
			}
		}
	}

	private string ApplyLoadout(LoadoutConfig loadout)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < loadout.spellIds.Length; i++)
		{
			string text = loadout.spellIds[i];
			Spell spell = RunManager.Instance.GetSpell(text);
			if (spell != null)
			{
				if (i == 0 && typeof(SpecialSpell).IsAssignableFrom(spell.GetType()))
				{
					SpecialSpell newSpell = UnityEngine.Object.Instantiate((SpecialSpell)spell);
					foreach (SpellCasting instance in SpellCasting.Instances)
					{
						foreach (Stance stance in instance.stances)
						{
							if (stance.enabled && stance.spellSlots.Count > 0)
							{
								SpellSlot spellSlot = stance.spellSlots[0];
								if (spellSlot.isUsable)
								{
									spellSlot.SetNewSpell(newSpell);
								}
							}
						}
					}
					list.Add("Special Spell: " + spell.name.value);
					continue;
				}
				bool flag = false;
				foreach (SpellCasting instance2 in SpellCasting.Instances)
				{
					foreach (Stance stance2 in instance2.stances)
					{
						if (!stance2.enabled)
						{
							continue;
						}
						foreach (SpellSlot spellSlot2 in stance2.spellSlots)
						{
							if (spellSlot2.spell == null && spellSlot2.isUsable)
							{
								Spell newSpell2 = UnityEngine.Object.Instantiate(spell);
								spellSlot2.SetNewSpell(newSpell2);
								list.Add("Spell: " + spell.name.value);
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					list.Add("<color=red>No available slots for spell: " + spell.name.value + "</color>");
				}
			}
			else
			{
				list.Add("<color=red>Failed to find spell: " + text + "</color>");
			}
		}
		string[] relicIds = loadout.relicIds;
		foreach (string text2 in relicIds)
		{
			Relic relicById = GetRelicById(text2);
			if (relicById != null)
			{
				RelicCollector.Instance.GetRelic(relicById);
				list.Add("Relic: " + relicById.name.value);
			}
			else
			{
				list.Add("<color=red>Failed to find relic: " + text2 + "</color>");
			}
		}
		return string.Join("\n", list);
	}

	private Relic GetRelicById(string relicId)
	{
		FieldInfo field = typeof(RunManager).GetField("allRelics", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			foreach (Relic item in (List<Relic>)field.GetValue(RunManager.Instance))
			{
				if (item.GetType().ToString() == relicId)
				{
					return item;
				}
			}
		}
		return null;
	}
}
