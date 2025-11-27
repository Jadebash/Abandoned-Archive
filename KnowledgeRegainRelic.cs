using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Knowledge Regain Relic")]
public class KnowledgeRegainRelic : Relic
{
	private RelicCollector relicCollector;

	public override void GainedRelic(GameObject player)
	{
		relicCollector = GameObject.Find("PlayerManager").GetComponent<RelicCollector>();
		relicCollector.OnSpendKnowledge += KnowledgeRegainRelic_OnSpendKnowledge;
	}

	private void KnowledgeRegainRelic_OnSpendKnowledge(int knowledge)
	{
		if (Random.Range(0f, 100f) <= 50f)
		{
			Use();
			float num = Random.Range(0f, 100f);
			if (num < 70f)
			{
				relicCollector.GainKnowledge(knowledge / 5);
			}
			else if (num < 85f)
			{
				relicCollector.GainKnowledge(knowledge / 3);
			}
			else if (num <= 100f)
			{
				relicCollector.GainKnowledge(knowledge / 2);
			}
		}
	}

	public override void LostRelic(GameObject player)
	{
		relicCollector.OnSpendKnowledge -= KnowledgeRegainRelic_OnSpendKnowledge;
	}
}
