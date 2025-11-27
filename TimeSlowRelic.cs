using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Time Slow Relic")]
public class TimeSlowRelic : Relic
{
	public GameObject visualEffect;

	public override void GainedRelic(GameObject player)
	{
		player.GetComponent<Health>().OnDamage += SlowTimeOnDamage;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<Health>().OnDamage -= SlowTimeOnDamage;
	}

	public void SlowTimeOnDamage(float damage, bool causedDeath, GameObject beingAttacked, GameObject attacker)
	{
		if (!causedDeath)
		{
			CoroutineEmitter.Instance.StartCoroutine(TimeSlow());
		}
	}

	private IEnumerator TimeSlow()
	{
		Use();
		GameObject visualEffectInstance = Object.Instantiate(visualEffect);
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds(0.4f);
		Time.timeScale = 1f;
		Object.Destroy(visualEffectInstance);
	}
}
