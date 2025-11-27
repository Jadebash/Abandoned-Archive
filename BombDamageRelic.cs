using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Bomb Damage Relic")]
public class BombDamageRelic : Relic
{
	public GameObject explosion;

	public override void GainedRelic(GameObject player)
	{
		player.GetComponent<Health>().OnDamage += ExplodeBomb;
		RunManager.Instance.OnKillEnemy += ExplodeOnKill;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<Health>().OnDamage -= ExplodeBomb;
		RunManager.Instance.OnKillEnemy -= ExplodeOnKill;
	}

	public void ExplodeBomb(float damage, bool causedDeath, GameObject beingAttacked, GameObject attacker)
	{
		Use();
		Object.Instantiate(explosion, beingAttacked.transform.position, Quaternion.identity);
		Screenshake.Instance.AddTrauma(0.8f);
	}

	public void ExplodeOnKill(GameObject attacker, GameObject enemy)
	{
		Use();
		Object.Instantiate(explosion, enemy.transform.position, Quaternion.identity);
		Screenshake.Instance.AddTrauma(0.8f);
	}
}
