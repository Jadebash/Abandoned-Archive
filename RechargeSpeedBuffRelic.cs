using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Recharge Speed Buff Relic")]
public class RechargeSpeedBuffRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		player.GetComponent<SpellCasting>().rechargeSpeedBuff = true;
		Use();
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<SpellCasting>().rechargeSpeedBuff = false;
	}
}
