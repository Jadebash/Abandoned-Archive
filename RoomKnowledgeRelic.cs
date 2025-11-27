using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Room Knowledge Relic")]
public class RoomKnowledgeRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		Use();
		player.GetComponent<SpellCasting>().roomKnowledgeRelic = true;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<SpellCasting>().roomKnowledgeRelic = false;
	}
}
