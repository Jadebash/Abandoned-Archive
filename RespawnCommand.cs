using UnityEngine;

[CreateAssetMenu(fileName = "Respawn Command", menuName = "Abandoned Archive/Debug/Commands/Respawn Command")]
public class RespawnCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (Manager.Instance == null)
		{
			return Fail(CommandFailType.PlayerNotFound);
		}
		Manager.Instance.Death();
		return Success("Respawned.");
	}
}
