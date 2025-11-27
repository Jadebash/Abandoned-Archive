using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Enemy Buff Relic")]
public class EnemyBuffRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		Use();
		player.GetComponent<SpellCasting>().enemyBuffRelic = true;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<SpellCasting>().enemyBuffRelic = false;
	}
}
