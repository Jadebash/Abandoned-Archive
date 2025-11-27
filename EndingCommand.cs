using UnityEngine;

[CreateAssetMenu(fileName = "Ending Command", menuName = "Abandoned Archive/Debug/Commands/Ending Command")]
public class EndingCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (args.Length == 0)
		{
			return Fail(CommandFailType.Custom, "Usage: ending <oblome|rodh>\nTriggers the specified ending sequence.");
		}
		if (args.Length > 1)
		{
			return Fail(CommandFailType.InvalidArgumentLength, "Usage: ending <oblome|rodh>");
		}
		if (Ending.Instance == null)
		{
			return Fail(CommandFailType.Custom, "Ending system not found! Make sure Ending.Instance exists in the scene.");
		}
		string text = args[0].ToLower();
		if (!(text == "oblome"))
		{
			if (text == "rodh")
			{
				if (DeveloperConsoleBehaviour.Instance != null)
				{
					DeveloperConsoleBehaviour.Instance.Toggle();
				}
				Ending.Instance.KillRodhEnding();
				return Success("Triggering Rodh ending...");
			}
			return Fail(CommandFailType.Custom, "Unknown ending type: '" + args[0] + "'. Use 'oblome' or 'rodh'.");
		}
		if (DeveloperConsoleBehaviour.Instance != null)
		{
			DeveloperConsoleBehaviour.Instance.Toggle();
		}
		Ending.Instance.KillOblomeEnding();
		return Success("Triggering Oblome ending...");
	}
}
