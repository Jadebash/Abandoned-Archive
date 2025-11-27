using Unity.Services.Analytics;
using UnityEngine;

public class PlayerAnalytics : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Health>().OnDamage += SubmitDamageAnalytics;
	}

	private void SubmitDamageAnalytics(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		if (AnalyticsManager.Instance != null)
		{
			PlayerDamageEvent e = new PlayerDamageEvent
			{
				Attacker = ((attacker == null) ? "null" : attacker.name),
				Damage = Mathf.RoundToInt(damage)
			};
			AnalyticsService.Instance.RecordEvent(e);
		}
	}
}
