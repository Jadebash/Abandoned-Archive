using UnityEngine;

public abstract class ConsoleCommand : ScriptableObject
{
	[SerializeField]
	private string commandWord = string.Empty;

	[SerializeField]
	[TextArea(2, 4)]
	private string description = "No description provided.";

	[SerializeField]
	[TextArea(2, 6)]
	private string usage = "";

	public string CommandWord => commandWord;

	public string Description => description;

	public string Usage => usage;

	public abstract CommandResponse Process(string[] args);

	public CommandResponse Fail(CommandFailType failType = CommandFailType.Custom, string response = "")
	{
		if (failType == CommandFailType.Custom)
		{
			return new CommandResponse(success: false, response);
		}
		return new CommandResponse(success: false, failType.ToString());
	}

	public CommandResponse Success(string response)
	{
		return new CommandResponse(success: true, response);
	}
}
