using FMODUnity;
using UnityEngine;

public class CrystalShrapnel : MonoBehaviour
{
	private Rigidbody2D rb;

	private float force;

	private Health[] enemies;

	private float destroyTimer = 4f;

	private bool stuck;

	private Vector3 rotation;

	private bool moving;

	[HideInInspector]
	public float bleedScale;

	private void Start()
	{
		CrystalShrapnel[] array = Object.FindObjectsOfType<CrystalShrapnel>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject != base.gameObject)
			{
				Physics2D.IgnoreCollision(array[i].GetComponent<Collider2D>(), GetComponent<Collider2D>());
			}
		}
		rotation = new Vector3(0f, 0f, Random.Range(0f, 360f));
		base.transform.rotation = Quaternion.Euler(rotation);
		force = Random.Range(5f, 10f);
		rb = GetComponent<Rigidbody2D>();
		enemies = Object.FindObjectsOfType<Health>();
		rb.AddForce(base.transform.up * force, ForceMode2D.Impulse);
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		destroyTimer -= Time.deltaTime;
		if (destroyTimer <= 0f)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/ShrapnelShatter", base.transform.position);
			Object.Destroy(base.gameObject);
		}
		if (stuck)
		{
			return;
		}
		rb.AddForce(base.transform.up * force * Time.deltaTime);
		Health[] array = enemies;
		foreach (Health health in array)
		{
			if (!(health == null))
			{
				if (!(health.gameObject.tag == "Core") && Vector2.Distance(health.gameObject.transform.position, base.gameObject.transform.position) <= 0.75f && health.gameObject.GetComponent<Movement>() == null)
				{
					health.Damage(2f);
					health.inflictBleed(bleedScale);
					Screenshake.Instance.AddTrauma(0.1f);
					RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/ShrapnelShatter", base.transform.position);
					base.transform.position = (health.gameObject.transform.position + base.transform.position) / 2f;
					Stick(health.gameObject);
				}
				continue;
			}
			break;
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if ((bool)col.gameObject.GetComponent<Bubble>())
		{
			col.gameObject.GetComponent<Bubble>().Pop();
		}
		else
		{
			HandleHit(col.transform.gameObject);
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
		Screenshake.Instance.AddTrauma(0.1f);
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/ShrapnelShatter", base.transform.position);
		Stick(hitObject);
		Health component = hitObject.GetComponent<Health>();
		if (component != null)
		{
			component.Damage(2f);
			component.inflictBleed(bleedScale);
		}
	}

	public void Stick(GameObject target)
	{
		stuck = true;
		rb.simulated = false;
		rb.velocity = Vector2.zero;
		base.transform.parent = target.transform;
	}
}
