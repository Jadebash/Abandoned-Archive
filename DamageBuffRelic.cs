using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Damage Buff Relic")]
public class DamageBuffRelic : Relic
{
	private SpellCasting spellCasting;

	public override void GainedRelic(GameObject player)
	{
		Use();
		spellCasting = player.GetComponent<SpellCasting>();
		spellCasting.damageBuffRelic = true;
	}

	public override void LostRelic(GameObject player)
	{
		spellCasting.damageBuffRelic = false;
	}
}
