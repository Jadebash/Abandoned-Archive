using UnityEngine;

public class DestroyOnBossDeath : MonoBehaviour
{
	public GameObject deathVFX;

	private void Start()
	{
		if (BossManager.Instance != null)
		{
			BossManager.Instance.OnBossDefeated += OnBossDefeated;
			if (!BossManager.Instance.inBossFight)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		if (BossManager.Instance != null)
		{
			BossManager.Instance.OnBossDefeated -= OnBossDefeated;
		}
	}

	private void OnBossDefeated()
	{
		if (deathVFX != null)
		{
			Object.Instantiate(deathVFX, base.transform.position, Quaternion.identity);
		}
		Object.Destroy(base.gameObject);
	}
}
