using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
	[Header("Damage Settings")]
	public float damage;

	public bool useDifficultyMultiplier = true;

	public bool sendAttacker = true;

	public bool ignorePlayer;

	[Header("Bounce Settings")]
	[Tooltip("How many times the projectile can bounce off surfaces before being destroyed.")]
	public int maxBounces;

	private int bounceCount;

	[Header("Tracking Settings")]
	[Range(0f, 100f)]
	[Tooltip("How strongly the projectile homes in on targets. 0 = no tracking, 100 = full tracking.")]
	public float trackingPercentage;

	[Tooltip("How far the projectile can detect potential targets to home towards.")]
	public float trackingRange = 5f;

	[Tooltip("Which layers this projectile is allowed to track. Objects not in these layers will be ignored.")]
	public LayerMask trackingLayers = -1;

	[Header("Effects")]
	public GameObject destructionParticles;

	private Rigidbody2D rb;

	private Transform target;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (trackingPercentage > 0f)
		{
			FindTargetIfNeeded();
			if (target != null)
			{
				Vector2 b = (target.position - base.transform.position).normalized;
				Vector2 normalized = rb.velocity.normalized;
				float t = Mathf.Clamp01(trackingPercentage / 100f);
				Vector2 normalized2 = Vector2.Lerp(normalized, b, t).normalized;
				rb.velocity = normalized2 * rb.velocity.magnitude;
			}
		}
	}

	private void FindTargetIfNeeded()
	{
		if (!(target == null) && !(Vector2.Distance(base.transform.position, target.position) > trackingRange))
		{
			return;
		}
		float num = float.PositiveInfinity;
		Transform transform = null;
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, trackingRange, trackingLayers);
		foreach (Collider2D collider2D in array)
		{
			if (collider2D.GetComponent<Health>() != null)
			{
				float num2 = Vector2.Distance(base.transform.position, collider2D.transform.position);
				if (num2 < num)
				{
					num = num2;
					transform = collider2D.transform;
				}
			}
		}
		target = transform;
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.isTrigger || !base.enabled || (ignorePlayer && col.tag == "Player"))
		{
			return;
		}
		Health component = col.GetComponent<Health>();
		if (component != null)
		{
			int num = 1;
			if (useDifficultyMultiplier && Difficulty.Instance != null)
			{
				num = Difficulty.Instance.damageMultiplier;
			}
			if (sendAttacker)
			{
				component.Damage(damage * (float)num, base.gameObject);
			}
			else
			{
				component.Damage(damage * (float)num);
			}
			DestroyProjectile();
			return;
		}
		if (bounceCount < maxBounces)
		{
			bounceCount++;
			Vector2 normalized = rb.velocity.normalized;
			Vector2 inNormal = (base.transform.position - col.bounds.ClosestPoint(base.transform.position)).normalized;
			Vector2 vector = Vector2.Reflect(normalized, inNormal);
			rb.velocity = vector * rb.velocity.magnitude;
		}
		else
		{
			DestroyProjectile();
		}
		DestroyBulletOnCollision component2 = col.GetComponent<DestroyBulletOnCollision>();
		if ((bool)component2 && component2.doDestroy)
		{
			base.enabled = false;
			Object.Destroy(base.gameObject);
		}
	}

	private void DestroyProjectile()
	{
		if ((bool)destructionParticles)
		{
			Object.Instantiate(destructionParticles, base.transform.position, Quaternion.identity);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, trackingRange);
	}
}
