using UnityEngine;

[CreateAssetMenu(fileName = "Add Knowledge Command", menuName = "Abandoned Archive/Debug/Commands/Add Knowledge Command")]
public class AddKnowledgeCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (args.Length != 1)
		{
			return Fail(CommandFailType.Custom, "Invalid number of arguments. Usage: addknowledge <amount>");
		}
		if (!int.TryParse(args[0], out var result))
		{
			return Fail(CommandFailType.Custom, "'" + args[0] + "' is not a valid number. Please provide an integer value.");
		}
		if (RelicCollector.Instance == null)
		{
			return Fail(CommandFailType.Custom, "Cannot add knowledge: Not in a run or RelicCollector not found.");
		}
		RelicCollector.Instance.GainKnowledge(result);
		return Success("Added " + result + " knowledge.");
	}
}
