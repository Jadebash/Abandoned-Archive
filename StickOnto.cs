using UnityEngine;

public class StickOnto : MonoBehaviour
{
	[HideInInspector]
	public float forceMultiplier = 1f;

	public float damage = 50f;

	private int hasPierced;

	public int pierce;

	public bool aimAssist = true;

	[Range(0f, 20f)]
	public float aimAssistAmount = 2f;

	[Range(0f, 180f)]
	public float aimAssistThreshold = 50f;

	public GameObject hitParticles;

	public bool disableTrailOnStick = true;

	public bool stick = true;

	public bool shakeOnDestroy;

	private Rigidbody2D rb;

	private GameObject[] allEnemies;

	private GameObject effect;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
	}

	private void FixedUpdate()
	{
		if (!aimAssist || allEnemies.Length == 0 || hasPierced != 0)
		{
			return;
		}
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		GameObject[] array = allEnemies;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 != null)
			{
				Vector2 to = gameObject2.transform.position - base.transform.position;
				float num2 = Vector2.Angle(base.transform.up, to);
				if (num2 < num)
				{
					num = num2;
					gameObject = gameObject2;
				}
			}
		}
		if (num < aimAssistThreshold && gameObject != null && Time.timeScale != 0f)
		{
			rb.velocity = Vector2.zero;
			rb.AddForce((gameObject.transform.position - base.transform.position).normalized * aimAssistAmount * 75f * Time.fixedDeltaTime * forceMultiplier, ForceMode2D.Impulse);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, gameObject.transform.position - base.transform.position, Vector3.forward), Vector3.forward), Time.fixedDeltaTime * aimAssistAmount);
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.transform.tag == "Enemy")
		{
			col.GetComponent<Health>()?.Damage(damage);
			if (pierce > hasPierced)
			{
				hasPierced++;
				return;
			}
			Screenshake.Instance.AddTrauma(0.4f);
			if (col.transform != null && stick)
			{
				if ((bool)base.transform.GetChild(0).GetComponent<SpriteRenderer>())
				{
					base.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Effects");
				}
				base.transform.position += new Vector3(rb.velocity.normalized.x * 0.35f, rb.velocity.normalized.y * 0.35f);
				base.transform.parent = col.transform;
				rb.simulated = false;
				Object.Instantiate(hitParticles, base.transform.position + base.transform.up * 0.15f, Quaternion.identity);
				base.enabled = false;
				if (disableTrailOnStick)
				{
					GetComponentInChildren<TrailRenderer>().enabled = false;
				}
			}
			else
			{
				Explode();
			}
		}
		else if ((col.gameObject.layer == 18 || col.gameObject.layer == 30) && !col.isTrigger)
		{
			Explode();
		}
		else if ((bool)col.GetComponent<Bubble>())
		{
			col.GetComponent<Bubble>().Pop();
		}
	}

	public void Explode()
	{
		effect = Object.Instantiate(hitParticles, base.transform.position + base.transform.up * 0.15f, Quaternion.identity);
		ModifyExplosionDamage();
		if (shakeOnDestroy)
		{
			Screenshake.Instance.AddTrauma(0.7f);
		}
		Object.Destroy(base.gameObject);
	}

	private void ModifyExplosionDamage()
	{
		if (GetComponent<OrbOfThunder>() != null)
		{
			effect.GetComponent<DamageOverTime>().damagePerSecond *= GetComponent<OrbOfThunder>().explosionDamageMoidifer;
		}
	}
}
