using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Enemy Heal Relic")]
public class EnemyHealRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		Use();
		player.GetComponent<SpellCasting>().enemyHealRelic = true;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<SpellCasting>().enemyHealRelic = false;
	}
}
