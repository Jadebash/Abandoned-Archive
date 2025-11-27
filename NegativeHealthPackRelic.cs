using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Negative Health Pack Relic")]
public class NegativeHealthPackRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		Use();
		player.GetComponent<Health>().negateHealthPack = true;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<Health>().negateHealthPack = false;
	}
}
