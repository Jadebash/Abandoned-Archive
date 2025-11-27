using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Relic Command", menuName = "Abandoned Archive/Debug/Commands/Spawn Relic Command")]
public class SpawnRelicCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (args.Length != 1)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: spawnrelic <relic_id> or spawnrelic --list");
		}
		if (args[0].Equals("--list", StringComparison.OrdinalIgnoreCase))
		{
			if (RunManager.Instance == null)
			{
				return Fail(CommandFailType.Custom, "Cannot list relics: Not in a run.");
			}
			string text = "<b>Available Relics:</b>\n";
			text += "────────────────────────────────────────\n";
			List<Relic> list = new List<Relic>();
			FieldInfo field = typeof(RunManager).GetField("allRelics", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				list = (List<Relic>)field.GetValue(RunManager.Instance);
			}
			if (list.Count == 0)
			{
				return Fail(CommandFailType.Custom, "No relics available in RunManager.");
			}
			foreach (Relic item in list)
			{
				string text2 = item.GetType().ToString();
				string text3 = ((!string.IsNullOrEmpty(item.name.value)) ? item.name.value : "Unknown");
				text = text + "<color=#00DDFF>" + text2 + "</color>\n  <color=#AAAAAA>" + text3 + "</color>\n\n";
			}
			text += "────────────────────────────────────────\n";
			text += $"<i>Total: {list.Count} relics</i>";
			return Success(text);
		}
		if (RunManager.Instance == null)
		{
			return Fail(CommandFailType.Custom, "Cannot spawn relic: Not in a run.");
		}
		Relic relic = RunManager.Instance.SpawnRelic(args[0]);
		if (relic == null)
		{
			return Fail(CommandFailType.Custom, "Relic '" + args[0] + "' not found. Use 'spawnrelic --list' to see available relics.");
		}
		RelicCollector.Instance.GainKnowledge(10);
		return Success("Spawned " + relic.name.value + ".");
	}
}
