using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
	public int damagePerHit;

	public float timeToDamage;

	public float gapTime;

	private float gapTimeSave;

	private void Start()
	{
		gapTimeSave = gapTime;
	}

	private void Update()
	{
		gapTime -= Time.deltaTime;
		timeToDamage -= Time.deltaTime;
		if (damagePerHit != 0 && timeToDamage != 0f && timeToDamage > 0f && gapTime <= 0f)
		{
			gapTime = gapTimeSave;
			timeToDamage -= Time.deltaTime;
			base.transform.gameObject.GetComponent<Health>().Damage(damagePerHit);
		}
		if (timeToDamage <= 0f)
		{
			Object.Destroy(this);
		}
	}
}
