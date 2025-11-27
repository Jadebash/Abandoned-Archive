using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Damage Boost Relic")]
public class DamageBoostRelic : Relic
{
	public override void GainedRelic(GameObject player)
	{
		SpellCasting component = player.GetComponent<SpellCasting>();
		component.damageBoostRelic = true;
		component.spellDamageMultiplier += 0.33f;
		Use();
	}

	public override void LostRelic(GameObject player)
	{
		SpellCasting component = player.GetComponent<SpellCasting>();
		component.damageBoostRelic = false;
		component.spellDamageMultiplier -= 0.33f;
	}
}
