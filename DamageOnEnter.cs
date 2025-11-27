using System.Collections.Generic;
using UnityEngine;

public class DamageOnEnter : MonoBehaviour
{
	public delegate void KillCallback(GameObject enemy);

	public delegate void HitCallback();

	public List<string> tagToDamage = new List<string>();

	public float damage = 20f;

	public bool useDifficultyMultiplier;

	public bool doKnockback;

	public float knockback = 750f;

	public bool doStun;

	public float stunTime = 0.5f;

	public bool shakeOnHit;

	public bool destroyOnDamage;

	public string animationTriggerOnDamage = "";

	public bool disableOnDamage;

	public bool killOnDamage;

	public float secondsUntilActive;

	public bool destroyProps = true;

	public float damageCooldown = 0.5f;

	public GameObject customAttacker;

	public bool useTrigger = true;

	public bool useCollision;

	public bool continuousDamage;

	public bool popBubbles = true;

	private float timer;

	private bool active = true;

	private Animator anim;

	private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>();

	public event KillCallback OnKill;

	public event HitCallback OnHit;

	private void Start()
	{
		if (secondsUntilActive > 0f)
		{
			active = false;
		}
		anim = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		if (secondsUntilActive > 0f && !active)
		{
			timer += Time.fixedDeltaTime;
			if (timer >= secondsUntilActive)
			{
				active = true;
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (useTrigger)
		{
			ProcessDamage(col);
		}
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (useCollision)
		{
			ProcessDamage(col.collider);
		}
	}

	public void OnTriggerStay2D(Collider2D col)
	{
		if (useTrigger && continuousDamage)
		{
			ProcessDamage(col);
		}
	}

	public void OnCollisionStay2D(Collision2D col)
	{
		if (useCollision && continuousDamage)
		{
			ProcessDamage(col.collider);
		}
	}

	private void ProcessDamage(Collider2D col)
	{
		if (!active)
		{
			return;
		}
		if (destroyProps && col.tag == "PropBreakable" && (bool)col.GetComponent<PropBreak>())
		{
			col.GetComponent<PropBreak>().Break();
		}
		if (popBubbles && (bool)col.GetComponent<Bubble>())
		{
			col.GetComponent<Bubble>().Pop();
		}
		if (!tagToDamage.Contains(col.transform.tag) || !col.GetComponent<Health>())
		{
			return;
		}
		GameObject gameObject = col.gameObject;
		if (gameObject == null || (lastDamageTime.ContainsKey(gameObject) && Time.time - lastDamageTime[gameObject] < damageCooldown))
		{
			return;
		}
		int num = 1;
		if (useDifficultyMultiplier && Difficulty.Instance != null)
		{
			num = Difficulty.Instance.damageMultiplier;
		}
		bool flag = false;
		flag = ((!(customAttacker != null)) ? col.GetComponent<Health>().Damage(Mathf.Round(damage * (float)num)) : col.GetComponent<Health>().Damage(Mathf.Round(damage * (float)num), customAttacker));
		lastDamageTime[gameObject] = Time.time;
		if (shakeOnHit)
		{
			Screenshake.Instance?.AddTrauma(0.8f);
		}
		this.OnHit?.Invoke();
		if (flag)
		{
			if (lastDamageTime.ContainsKey(gameObject))
			{
				lastDamageTime.Remove(gameObject);
			}
			this.OnKill?.Invoke(col.gameObject);
		}
		if ((bool)col.GetComponent<Enemy>() && doKnockback)
		{
			if (!doStun)
			{
				col.GetComponent<Enemy>().Stun(0.1f);
			}
			col.GetComponent<Rigidbody2D>().AddForce((base.transform.position - col.transform.position).normalized * (0f - knockback));
		}
		if ((bool)col.GetComponent<Enemy>() && doStun)
		{
			col.GetComponent<Enemy>().Stun(stunTime);
		}
		if (destroyOnDamage)
		{
			Object.Destroy(base.gameObject);
		}
		if (disableOnDamage)
		{
			base.gameObject.SetActive(value: false);
		}
		if (killOnDamage)
		{
			GetComponent<Health>().Damage(999999f, base.gameObject, ignoreInvincibility: true);
		}
		if (animationTriggerOnDamage != "")
		{
			anim.SetTrigger(animationTriggerOnDamage);
		}
	}
}
