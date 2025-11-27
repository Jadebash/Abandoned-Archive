using UnityEngine;

[CreateAssetMenu(fileName = "Clear Command", menuName = "Abandoned Archive/Debug/Commands/Clear Command")]
public class ClearCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		DeveloperConsoleBehaviour.Instance.ClearLog();
		return Success("");
	}
}
