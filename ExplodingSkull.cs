using FMODUnity;
using UnityEngine;

public class ExplodingSkull : MonoBehaviour
{
	private float explosionTimer;

	private GameObject player;

	private float freezeTimer;

	private Rigidbody2D rb;

	public GameObject particles;

	public LayerMask obstacleLayers;

	private bool moving;

	private float explosionRadius = 1.25f;

	private bool CanDamagePlayer(float maxDistance)
	{
		if (player == null)
		{
			return false;
		}
		Vector2 vector = player.transform.position - base.transform.position;
		float magnitude = vector.magnitude;
		if (magnitude > maxDistance)
		{
			return false;
		}
		if (magnitude <= 0.001f)
		{
			return true;
		}
		int num = obstacleLayers.value;
		if (num == 0)
		{
			num = LayerMask.GetMask("Room", "BossRoom", "Barrier");
		}
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, vector.normalized, magnitude, num);
		if (raycastHit2D.collider != null)
		{
			if (raycastHit2D.collider.transform == player.transform || raycastHit2D.collider.transform.IsChildOf(player.transform))
			{
				return true;
			}
			return false;
		}
		return true;
	}

	private void Start()
	{
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		GetComponent<SpriteRenderer>().color = Color.white;
		if (player != null)
		{
			Vector3 vector = player.transform.position - base.transform.position;
			rb.AddForce(vector * 3f, ForceMode2D.Impulse);
			moving = true;
		}
	}

	private void Update()
	{
		explosionTimer += Time.deltaTime;
		freezeTimer += Time.deltaTime;
		if (moving && player != null)
		{
			if (Vector2.Distance(player.transform.position, base.gameObject.transform.position) <= 0.5f)
			{
				Explode();
			}
			if (explosionTimer >= 2f)
			{
				Explode();
			}
		}
		else if (explosionTimer >= 2f)
		{
			if (player != null)
			{
				Explode();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void Explode()
	{
		if (CanDamagePlayer(explosionRadius))
		{
			int num = 1;
			if (Difficulty.Instance != null)
			{
				num = Difficulty.Instance.damageMultiplier;
			}
			player.GetComponent<Health>().Damage(20f * (float)num);
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/HeadExplode", base.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
		if (particles != null)
		{
			Object.Instantiate(particles, base.transform.position, Quaternion.identity);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnCollisionEnter2D()
	{
		if (moving)
		{
			Explode();
		}
	}
}
