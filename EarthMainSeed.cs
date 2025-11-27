using FMODUnity;
using UnityEngine;

public class EarthMainSeed : MonoBehaviour
{
	private Rigidbody2D rb;

	private SpellCasting player;

	[SerializeField]
	public float damage;

	public float speed = 5f;

	public GameObject vines;

	public GameObject explosionParticles;

	private void Start()
	{
		EarthMainSeed[] array = Object.FindObjectsOfType<EarthMainSeed>();
		foreach (EarthMainSeed earthMainSeed in array)
		{
			if (earthMainSeed.gameObject != base.gameObject)
			{
				Physics2D.IgnoreCollision(GetComponent<Collider2D>(), earthMainSeed.gameObject.GetComponent<Collider2D>());
			}
		}
		player = PlayerManager.Instance.ClosestPlayerSpellCaster(base.gameObject.transform.position);
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(base.transform.up * speed * 1.3f, ForceMode2D.Impulse);
	}

	public void SetDamage(float DamageSet)
	{
		damage = DamageSet;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if ((bool)col.gameObject.GetComponent<Bubble>())
		{
			col.gameObject.GetComponent<Bubble>().Pop();
		}
		else
		{
			HandleHit(col.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Enemy") || col.CompareTag("PropBreakable"))
		{
			HandleHit(col.gameObject);
		}
	}

	private void HandleHit(GameObject hitObject)
	{
		Health component = hitObject.GetComponent<Health>();
		if (component != null && hitObject.tag != "Core" && hitObject.GetComponent<Movement>() == null)
		{
			Enemy component2 = hitObject.GetComponent<Enemy>();
			if (component2 != null)
			{
				if (component2.movementSpeed > 1f)
				{
					component2.movementSpeed -= 0.25f;
				}
				else if (!Object.FindObjectOfType<EarthMiniVines>())
				{
					Object.Instantiate(vines, hitObject.transform.position, Quaternion.identity);
					component2.movementSpeed = 2f;
					RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Vines_Growing", base.transform.position);
					component2.Stun(4f);
				}
			}
			component.Damage(damage * player.spellDamageMultiplier, player.gameObject);
		}
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Earth_Main_Destroy", base.transform.position);
		Object.Instantiate(explosionParticles, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
		Screenshake.Instance.AddTrauma(0.3f);
	}
}
