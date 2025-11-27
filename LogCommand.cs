using UnityEngine;

[CreateAssetMenu(fileName = "Log Command", menuName = "Abandoned Archive/Debug/Commands/Log Command")]
public class LogCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		string text = string.Join(" ", args);
		Debug.Log(text);
		return Success(text);
	}
}
