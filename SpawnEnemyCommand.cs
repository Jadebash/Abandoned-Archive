using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Enemy Command", menuName = "Abandoned Archive/Debug/Commands/Spawn Enemy Command")]
public class SpawnEnemyCommand : ConsoleCommand
{
	public List<GameObject> enemies = new List<GameObject>();

	public override CommandResponse Process(string[] args)
	{
		if (args.Length != 1)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: spawnenemy <enemy_name> or spawnenemy --list");
		}
		if (args[0].Equals("--list", StringComparison.OrdinalIgnoreCase))
		{
			if (enemies.Count == 0)
			{
				return Fail(CommandFailType.Custom, "No enemies configured in this command.");
			}
			string text = "<b>Available Enemies:</b>\n";
			text += "────────────────────────────────────────\n";
			foreach (GameObject enemy in enemies)
			{
				if (enemy != null)
				{
					text = text + "<color=#00DDFF>" + enemy.name + "</color>\n\n";
				}
			}
			text += "────────────────────────────────────────\n";
			text += $"<i>Total: {enemies.Count} enemies</i>";
			return Success(text);
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
		if (gameObject == null)
		{
			return Fail(CommandFailType.Custom, "Player not found in scene.");
		}
		foreach (GameObject enemy2 in enemies)
		{
			if (enemy2.name.Equals(args[0], StringComparison.OrdinalIgnoreCase))
			{
				UnityEngine.Object.Instantiate(enemy2, gameObject.transform.position, Quaternion.identity);
				return Success("Spawned " + enemy2.name + ".");
			}
		}
		return Fail(CommandFailType.Custom, "Enemy '" + args[0] + "' not found. Use 'spawnenemy --list' to see available enemies.");
	}
}
