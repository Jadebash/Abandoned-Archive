using UnityEngine;

[CreateAssetMenu(fileName = "Teleport to Boss Command", menuName = "Abandoned Archive/Debug/Commands/Teleport to Boss Command")]
public class TPBossCommand : ConsoleCommand
{
	public override CommandResponse Process(string[] args)
	{
		if (MapGenerator.Instance == null)
		{
			return Fail(CommandFailType.PlayerNotFound);
		}
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject in players)
		{
			if (gameObject == null)
			{
				return Fail(CommandFailType.PlayerNotFound);
			}
			Movement component = gameObject.GetComponent<Movement>();
			if (component != null)
			{
				component.SafeTeleport(MapGenerator.Instance.bossRoomGenerationInformation.exitPosition + new Vector3(0f, 3f, 0f));
			}
		}
		RoomOptimiser[] array = Object.FindObjectsOfType<RoomOptimiser>(includeInactive: true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Optimise(null);
		}
		ShadowOptimiser[] array2 = Object.FindObjectsOfType<ShadowOptimiser>();
		LightOptimiser[] array3 = Object.FindObjectsOfType<LightOptimiser>();
		ShadowCasterOptimiser[] array4 = Object.FindObjectsOfType<ShadowCasterOptimiser>();
		ShadowOptimiser[] array5 = array2;
		for (int i = 0; i < array5.Length; i++)
		{
			array5[i].UpdateLight();
		}
		LightOptimiser[] array6 = array3;
		for (int i = 0; i < array6.Length; i++)
		{
			array6[i].UpdateLight();
		}
		ShadowCasterOptimiser[] array7 = array4;
		for (int i = 0; i < array7.Length; i++)
		{
			array7[i].UpdateCaster();
		}
		return Success("Teleported to boss room.");
	}
}
