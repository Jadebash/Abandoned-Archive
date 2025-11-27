using FMODUnity;
using UnityEngine;

public class CrystalOrb : MonoBehaviour
{
	public float explosionTimer;

	public GameObject projectile;

	[HideInInspector]
	public bool thrown;

	private Health[] enemies;

	public float damage = 12f;

	private SpellCasting player;

	[HideInInspector]
	public int shrapnelCount = 4;

	[HideInInspector]
	public float damageScale;

	[HideInInspector]
	public float bleedScale;

	private bool soundEffect;

	private void Start()
	{
		enemies = Object.FindObjectsOfType<Health>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position).GetComponent<SpellCasting>();
	}

	private void Update()
	{
		if (thrown)
		{
			if (!GetComponent<Collider2D>().enabled)
			{
				GetComponent<Collider2D>().enabled = true;
			}
			if (!soundEffect)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/CrystalMove", base.transform.position);
				soundEffect = true;
			}
			explosionTimer -= Time.deltaTime;
			if (explosionTimer <= 0f)
			{
				explode();
			}
			Health[] array = enemies;
			foreach (Health health in array)
			{
				if (!(health == null) && !(health.gameObject.tag == "Core") && Vector2.Distance(health.gameObject.transform.position, base.gameObject.transform.position) <= 2f && health.gameObject.GetComponent<Movement>() == null)
				{
					health.Damage(damage * player.spellDamageMultiplier * damageScale, player.gameObject);
					explode();
				}
			}
		}
		else
		{
			GetComponent<Collider2D>().enabled = false;
			base.transform.position = player.gameObject.transform.position;
		}
	}

	private void explode()
	{
		Screenshake.Instance.AddTrauma(0.5f);
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/CrystalShatter", base.transform.position);
		Object.Destroy(base.gameObject);
		float num = 0f;
		for (int i = 0; i < shrapnelCount; i++)
		{
			Object.Instantiate(projectile, base.gameObject.transform.position, Quaternion.Euler(new Vector3(0f, 0f, num)));
			projectile.GetComponent<CrystalShrapnel>().bleedScale = bleedScale;
			num += 45f;
		}
	}
}
