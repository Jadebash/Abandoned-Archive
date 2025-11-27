using UnityEngine;

[CreateAssetMenu(fileName = "Godmode Command", menuName = "Abandoned Archive/Debug/Commands/Godmode Command")]
public class GodCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (PlayerManager.players == null || PlayerManager.players.Length == 0)
		{
			return Fail(CommandFailType.Custom, "No players found in the scene.");
		}
		string text = "";
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject == null)
			{
				return Fail(CommandFailType.Custom, "Player object is null.");
			}
			Health component = gameObject.GetComponent<Health>();
			if (component == null)
			{
				return Fail(CommandFailType.Custom, "Player does not have a Health component.");
			}
			component.godmode = !component.godmode;
			text = text + gameObject.name + " is now " + (component.godmode ? "in " : "out of ") + "godmode.\n";
		}
		return Success(text);
	}
}
