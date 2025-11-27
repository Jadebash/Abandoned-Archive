using UnityEngine;

[CreateAssetMenu(fileName = "Help Command", menuName = "Abandoned Archive/Debug/Commands/Help Command")]
public class HelpCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (args.Length == 0)
		{
			return Success(DeveloperConsoleBehaviour.Instance.HelpMessage());
		}
		if (args.Length == 1)
		{
			return Success(DeveloperConsoleBehaviour.Instance.GetCommandHelp(args[0]));
		}
		return Fail(CommandFailType.InvalidArgumentLength, "Usage: help [command]");
	}
}
