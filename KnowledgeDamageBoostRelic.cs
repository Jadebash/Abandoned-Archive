using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Knowledge Damage Boost Relic")]
public class KnowledgeDamageBoostRelic : Relic
{
	private SpellCasting spellCasting;

	private RelicCollector relicCollector;

	private float knowledgeEquation(int knowledge)
	{
		return (float)knowledge / 300f;
	}

	public override void GainedRelic(GameObject player)
	{
		relicCollector = GameObject.Find("PlayerManager").GetComponent<RelicCollector>();
		relicCollector.OnGainKnowledge += KnowledgeDamageBoostRelic_OnGainKnowledge;
		relicCollector.OnSpendKnowledge += KnowledgeDamageBoostRelic_OnSpendKnowledge;
		spellCasting = player.GetComponent<SpellCasting>();
		spellCasting.spellDamageMultiplier += knowledgeEquation(relicCollector.knowledge);
		Use();
	}

	private void KnowledgeDamageBoostRelic_OnSpendKnowledge(int knowledge)
	{
		spellCasting.spellDamageMultiplier -= knowledgeEquation(knowledge);
	}

	private void KnowledgeDamageBoostRelic_OnGainKnowledge(int knowledge)
	{
		spellCasting.spellDamageMultiplier += knowledgeEquation(knowledge);
	}

	public override void LostRelic(GameObject player)
	{
		relicCollector.OnGainKnowledge -= KnowledgeDamageBoostRelic_OnGainKnowledge;
		relicCollector.OnSpendKnowledge -= KnowledgeDamageBoostRelic_OnSpendKnowledge;
		spellCasting.spellDamageMultiplier -= knowledgeEquation(relicCollector.knowledge);
	}
}
