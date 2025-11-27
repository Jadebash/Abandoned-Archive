using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Damage Health Relic")]
public class DamageHealthRelic : Relic
{
	private Health health;

	private SpellCasting spellCasting;

	public override void GainedRelic(GameObject player)
	{
		Use();
		health = player.GetComponent<Health>();
		spellCasting = player.GetComponent<SpellCasting>();
		health.damageModifier += 1f;
		spellCasting.spellDamageMultiplier += 0.5f;
	}

	public override void LostRelic(GameObject player)
	{
		health.damageModifier -= 1f;
		spellCasting.spellDamageMultiplier -= 0.5f;
	}
}
