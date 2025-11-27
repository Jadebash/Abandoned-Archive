using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Kill Speed Buff Relic")]
public class KillSpeedBuffRelic : Relic
{
	private Movement playerMovement;

	public override void GainedRelic(GameObject player)
	{
		playerMovement = player.GetComponent<Movement>();
		RunManager.Instance.OnKillEnemy += KilledEnemy;
	}

	public override void LostRelic(GameObject player)
	{
		RunManager.Instance.OnKillEnemy -= KilledEnemy;
	}

	public void KilledEnemy(GameObject attacker, GameObject enemyKilled)
	{
		Use();
		CoroutineEmitter.Instance.StartCoroutine(IncreaseMovementSpeed());
	}

	private IEnumerator IncreaseMovementSpeed()
	{
		playerMovement.AddSpeedEffect(0.1f, 3f, "KillSpeedBuffRelic");
		yield return new WaitForSeconds(3f);
	}
}
