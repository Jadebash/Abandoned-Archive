using UnityEngine;

[CreateAssetMenu(fileName = "Print Save Command", menuName = "Abandoned Archive/Debug/Commands/Print Save Command")]
public class PrintSaveCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		return Success(SaveSystem.SaveToString(SaveManager.Instance.currentSave));
	}
}
