using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Faster Roll Relic")]
public class FasterRollRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		player.GetComponent<Movement>().OnRoll += Roll;
		player.GetComponent<Movement>().rollSpeedModifier += 0.5f;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<Movement>().OnRoll -= Roll;
		player.GetComponent<Movement>().rollSpeedModifier -= 0.5f;
	}

	public void Roll()
	{
		Use();
	}
}
