using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Health Pack Relic")]
public class HealthPackRelic : Relic
{
	private Health health;

	public float chance = 25f;

	public override void GainedRelic(GameObject player)
	{
		health = player.GetComponent<Health>();
		health.OnHeal += PlayerHeal;
	}

	public override void LostRelic(GameObject player)
	{
		health.OnHeal -= PlayerHeal;
	}

	public void PlayerHeal(float amount, bool healthPack)
	{
		if (healthPack && Random.Range(0f, 100f) < chance)
		{
			Use();
			health.Heal(amount);
		}
	}
}
