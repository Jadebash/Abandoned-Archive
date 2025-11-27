using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Low Vision Relic")]
public class LowVisionRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		Use();
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.reduceVision();
		}
	}

	public override void LostRelic(GameObject player)
	{
		foreach (SpellCasting instance in SpellCasting.Instances)
		{
			instance.increaseVision();
		}
	}
}
