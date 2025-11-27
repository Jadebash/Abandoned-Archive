using System.Collections;
using UnityEngine;

public class Bubble : MonoBehaviour
{
	public Health target;

	private Rigidbody2D targetRB;

	private Enemy targetEnemy;

	private Boss targetBoss;

	private Rigidbody2D rb;

	public float damagePerSecond = 2f;

	public float tickLength = 1f;

	public float duration = 2f;

	public float floatSpeed = 2f;

	public float floatStrength = 1f;

	public float damageModifierIncrease;

	public float burstDamage;

	public GameObject burstPrefab;

	private bool popping;

	private float damageTimer;

	private float durationTimer;

	private float noiseOffsetX;

	private float noiseOffsetY;

	private Transform originalParent;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		targetRB = target.GetComponent<Rigidbody2D>();
		targetEnemy = target.GetComponent<Enemy>();
		targetBoss = target.GetComponent<Boss>();
		target.OnDamage += Target_OnDamage;
		target.damageModifier += damageModifierIncrease;
		base.transform.position = target.transform.position;
		if (targetBoss == null)
		{
			originalParent = target.transform.parent;
			target.transform.parent = base.transform;
		}
		else
		{
			base.transform.parent = target.transform;
			targetBoss.GetComponent<Animator>().speed = 0.5f;
			rb.simulated = false;
		}
		durationTimer = duration;
		noiseOffsetX = Random.Range(0f, 1000f);
		noiseOffsetY = Random.Range(0f, 1000f);
		if (targetRB != null && targetBoss == null)
		{
			targetRB.velocity = new Vector2(0f, 0f);
			targetRB.angularVelocity = 0f;
			targetRB.simulated = false;
		}
		if (targetEnemy != null)
		{
			targetEnemy.Stun(duration);
		}
	}

	private void Target_OnDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker)
	{
		if (!popping && (!(attacker == base.gameObject) || causedDeath))
		{
			Pop(!causedDeath);
		}
	}

	private void Update()
	{
		damageTimer -= Time.deltaTime;
		if (damageTimer <= 0f)
		{
			damageTimer += tickLength;
			target.Damage(Mathf.RoundToInt(damagePerSecond * tickLength), base.gameObject);
		}
		durationTimer -= Time.deltaTime;
		if (durationTimer <= 0f)
		{
			Pop();
		}
	}

	private void FixedUpdate()
	{
		if (rb != null && !popping && targetBoss == null)
		{
			float num = Time.time * floatSpeed;
			float x = Mathf.PerlinNoise(num + noiseOffsetX, noiseOffsetX) * 2f - 1f;
			float y = Mathf.PerlinNoise(num + noiseOffsetY, noiseOffsetY) * 2f - 1f;
			Vector2 normalized = new Vector2(x, y).normalized;
			rb.velocity = normalized * floatStrength;
		}
	}

	public void Pop(bool doDamage = true)
	{
		popping = true;
		if (target == null)
		{
			Destroy();
			return;
		}
		if (targetEnemy != null && doDamage)
		{
			targetEnemy.Unstun();
		}
		target.OnDamage -= Target_OnDamage;
		target.StartCoroutine(ReduceDamageModifier());
		if (targetBoss == null)
		{
			target.transform.parent = originalParent;
			if ((bool)target.transform.parent?.GetComponent<Bubble>())
			{
				target.transform.parent = null;
			}
		}
		if (doDamage)
		{
			int num = Mathf.RoundToInt(burstDamage);
			if (num > 0)
			{
				target.Damage(num, base.gameObject);
			}
		}
		if (targetRB != null && targetBoss == null)
		{
			targetRB.velocity = new Vector2(0f, 0f);
			targetRB.angularVelocity = 0f;
			targetRB.simulated = true;
		}
		if (targetBoss != null)
		{
			targetBoss.GetComponent<Animator>().speed = 1f;
		}
		Destroy();
	}

	private void Destroy()
	{
		Object.Instantiate(burstPrefab, base.transform.position, Quaternion.identity);
		Screenshake.Instance.AddTrauma(0.1f);
		Object.Destroy(base.gameObject);
	}

	private IEnumerator ReduceDamageModifier()
	{
		yield return new WaitForSeconds(0.1f);
		target.damageModifier -= damageModifierIncrease;
	}
}
