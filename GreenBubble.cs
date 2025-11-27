using UnityEngine;

public class GreenBubble : MonoBehaviour
{
	private GameObject player;

	public float speed = 5f;

	private int lives = 2;

	private float hitCooldown;

	private float separationDistance = 2f;

	private float separationForce = 6f;

	private float orbitDirection;

	public bool canMove = true;

	private void Start()
	{
		player = PlayerManager.ClosestPlayer(base.transform.position);
		orbitDirection = ((Random.value > 0.5f) ? 1f : (-1f));
	}

	private void Update()
	{
		if (player == null)
		{
			return;
		}
		hitCooldown -= Time.deltaTime;
		if (!canMove)
		{
			return;
		}
		Vector3 normalized = (player.transform.position - base.transform.position).normalized;
		Vector3 vector = new Vector3(0f - normalized.y, normalized.x, 0f) * orbitDirection;
		Vector3 vector2 = normalized;
		Vector3 zero = Vector3.zero;
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, separationDistance);
		foreach (Collider2D collider2D in array)
		{
			if (collider2D.gameObject != base.gameObject && (bool)collider2D.GetComponent<GreenBubble>())
			{
				Vector3 vector3 = base.transform.position - collider2D.transform.position;
				zero += vector3.normalized / (vector3.magnitude + 0.01f);
			}
		}
		Vector3 vector4 = vector2 * 1.4f + vector * 0.7f + zero * separationForce;
		vector4.Normalize();
		base.transform.position += vector4 * speed * Time.deltaTime;
		float num = 0.5f;
		if (!(Vector3.Distance(base.transform.position, player.transform.position) < num))
		{
			return;
		}
		Movement component = player.GetComponent<Movement>();
		if (!(component != null))
		{
			return;
		}
		if (component.rolling)
		{
			if (hitCooldown <= 0f)
			{
				TakeHit();
			}
		}
		else
		{
			player.GetComponent<Health>().Damage(20f);
		}
	}

	private void TakeHit()
	{
		lives--;
		hitCooldown = 0.5f;
		if (lives <= 0)
		{
			GetComponent<Animator>().SetTrigger("Pop");
		}
		else
		{
			base.transform.localScale *= 0.75f;
		}
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
