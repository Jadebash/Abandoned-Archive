using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Roll Cancel Relic")]
public class RollCancelRelic : Relic
{
	private List<Movement> movements = new List<Movement>();

	public override void GainedRelic(GameObject player)
	{
		Movement component = player.GetComponent<Movement>();
		if (component != null && !movements.Contains(component))
		{
			movements.Add(component);
			component.rollCancel = true;
		}
	}

	public override void LostRelic(GameObject player)
	{
		Movement component = player.GetComponent<Movement>();
		if (component != null && movements.Contains(component))
		{
			component.rollCancel = false;
			movements.Remove(component);
		}
	}
}
