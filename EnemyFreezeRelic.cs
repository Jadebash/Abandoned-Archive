using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Enemy Freeze Relic")]
public class EnemyFreezeRelic : Relic
{
	private RunManager runManager;

	private GameObject player;

	private Enemy enemyVar;

	public GameObject rose;

	public override void GainedRelic(GameObject player)
	{
		runManager = GameObject.Find("PlayerManager").GetComponent<RunManager>();
		runManager.OnKillEnemy += RunManager_OnKillEnemy;
	}

	private void RunManager_OnKillEnemy(GameObject attacker = null, GameObject enemy = null)
	{
		if (Random.Range(0f, 100f) < 15f)
		{
			Use();
			Object.Instantiate(rose, enemy.transform.position, Quaternion.identity);
		}
	}

	public override void LostRelic(GameObject player)
	{
		runManager.OnKillEnemy -= RunManager_OnKillEnemy;
	}
}
