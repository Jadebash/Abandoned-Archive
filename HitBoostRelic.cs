using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Hit Boost Relic")]
public class HitBoostRelic : Relic
{
	private Movement movement;

	private Health health;

	private float ogInvincibilityTime;

	public override void GainedRelic(GameObject player)
	{
		movement = player.GetComponent<Movement>();
		health = player.GetComponent<Health>();
		ogInvincibilityTime = health.invincibilityTime;
		health.OnDamage += Health_OnDamage;
	}

	private IEnumerator yes()
	{
		movement.AddSpeedEffect(0.5f, 1f, "HitBoostRelic");
		health.invincibilityTime = 2.5f;
		yield return new WaitForSeconds(1f);
		health.invincibilityTime = ogInvincibilityTime;
	}

	private void Health_OnDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		Use();
		CoroutineEmitter.Instance.StartCoroutine(yes());
	}

	public override void LostRelic(GameObject player)
	{
		health.OnDamage -= Health_OnDamage;
	}
}
