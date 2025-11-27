using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Knowledge Health Relic")]
public class KnowledgeHealthRelic : Relic
{
	private RelicCollector relicCollector;

	private Health health;

	public override void GainedRelic(GameObject player)
	{
		health = player.GetComponent<Health>();
		relicCollector = GameObject.Find("PlayerManager").GetComponent<RelicCollector>();
		relicCollector.knowledgeGainModifier += 0.5f;
		health.maxHealth -= 40f;
		health.ChangeHealthUI();
		Use();
		if (health.health >= health.maxHealth)
		{
			health.health = health.maxHealth;
		}
	}

	public override void LostRelic(GameObject player)
	{
		relicCollector.knowledgeGainModifier -= 0.5f;
		health.maxHealth += 40f;
		health.ChangeHealthUI();
	}
}
