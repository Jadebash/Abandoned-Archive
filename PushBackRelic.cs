using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Push Back Relic")]
public class PushBackRelic : Relic
{
	private Health health;

	public override void GainedRelic(GameObject player)
	{
		Use();
		health = player.GetComponent<Health>();
		health.pushBack = true;
	}

	public override void LostRelic(GameObject player)
	{
		health.pushBack = false;
	}
}
