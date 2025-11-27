using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Spell Fail Relic")]
public class SpellFailRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		Use();
		player.GetComponent<SpellCasting>().spellFailRelic = true;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<SpellCasting>().spellFailRelic = false;
	}
}
