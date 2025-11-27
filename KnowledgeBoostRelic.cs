using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Knowledge Boost Relic")]
public class KnowledgeBoostRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		GameObject.Find("PlayerManager").GetComponent<RelicCollector>().knowledgeGainModifier = 1.1f;
		Use();
	}

	public override void LostRelic(GameObject player)
	{
		GameObject.Find("PlayerManager").GetComponent<RelicCollector>().knowledgeGainModifier = 1f;
	}
}
