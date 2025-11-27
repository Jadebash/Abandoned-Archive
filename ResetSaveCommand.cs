using UnityEngine;

[CreateAssetMenu(fileName = "Reset Save Command", menuName = "Abandoned Archive/Debug/Commands/Reset Save Command")]
public class ResetSaveCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		SaveManager.Instance.currentSave = new Save();
		SaveManager.Instance.LoadSaveSettings();
		return Success("Reset save and loaded default settings.");
	}
}
