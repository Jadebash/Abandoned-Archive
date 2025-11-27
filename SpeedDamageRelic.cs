using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Speed Damage Relic")]
public class SpeedDamageRelic : Relic
{
	private SpellCasting spellCasting;

	public override void GainedRelic(GameObject player)
	{
		Use();
		spellCasting = player.GetComponent<SpellCasting>();
		spellCasting.speedDamage = true;
	}

	public override void LostRelic(GameObject player)
	{
		spellCasting.speedDamage = false;
	}
}
