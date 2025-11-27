using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Kill Boost Relic")]
public class KillBoostRelic : Relic
{
	private GameObject player;

	private int enemiesKilled;

	private int statBoostCount;

	public override void GainedRelic(GameObject player)
	{
		RunManager.Instance.OnKillEnemy += Instance_OnKillEnemy;
		this.player = player;
	}

	private void Instance_OnKillEnemy(GameObject attacker = null, GameObject enemy = null)
	{
		enemiesKilled++;
		CoroutineEmitter.Instance.StartCoroutine(StatBoost());
	}

	private IEnumerator StatBoost()
	{
		int currentEnemiesKilled = enemiesKilled;
		if (enemiesKilled > 1)
		{
			statBoostCount++;
			player.GetComponent<Movement>().AddSpeedEffect(0.1f, 3f, "KillBoostRelic");
			player.GetComponent<SpellCasting>().spellDamageMultiplier += 0.1f;
			RelicCollector.Instance.knowledgeGainModifier += 0.1f;
			Use();
		}
		yield return new WaitForSeconds(3f);
		if (enemiesKilled == currentEnemiesKilled)
		{
			player.GetComponent<Movement>().RemoveSpeedEffect("KillBoostRelic", removeAll: true);
			player.GetComponent<SpellCasting>().spellDamageMultiplier -= 0.1f * (float)statBoostCount;
			RelicCollector.Instance.knowledgeGainModifier -= 0.1f * (float)statBoostCount;
			enemiesKilled = 0;
			statBoostCount = 0;
		}
	}

	public override void LostRelic(GameObject player)
	{
		RunManager.Instance.OnKillEnemy -= Instance_OnKillEnemy;
	}
}
