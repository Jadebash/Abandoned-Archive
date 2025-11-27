using UnityEngine;

public class RufusArm : MonoBehaviour
{
	private Rigidbody2D rb;

	public float speed;

	[HideInInspector]
	public Vector3 currentVel;

	private GameObject rufus;

	[HideInInspector]
	public Vector3 direction;

	public float hitTimer;

	[SerializeField]
	private float verticalImpulse = 5f;

	[SerializeField]
	private float minVerticalSpeed = 2.5f;

	[SerializeField]
	private float horizontalImpulse = 5f;

	[SerializeField]
	private float minHorizontalSpeed = 2.5f;

	private void Start()
	{
		hitTimer = 0f;
		rufus = Object.FindObjectOfType<Rufus>().gameObject;
		rb = GetComponent<Rigidbody2D>();
		GameObject gameObject = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		if (gameObject != null)
		{
			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
		}
	}

	private void Update()
	{
		if (rufus == null)
		{
			Object.Destroy(base.gameObject);
		}
		hitTimer += Time.deltaTime;
		if (hitTimer >= 10f)
		{
			base.transform.position = rufus.transform.position;
			hitTimer = 0f;
		}
		currentVel = rb.velocity;
	}

	private void FixedUpdate()
	{
		if (Time.timeScale != 0f)
		{
			rb.AddForce(direction * speed);
			if (rb.velocity.magnitude >= 14f)
			{
				rb.AddForce(-rb.velocity);
			}
			Vector2 velocity = rb.velocity;
			bool flag = false;
			Vector2 zero = Vector2.zero;
			if (Mathf.Abs(velocity.y) < minVerticalSpeed && Mathf.Abs(velocity.y) > 0.01f)
			{
				float num = ((velocity.y >= 0f) ? 1f : (-1f));
				zero.y = num * verticalImpulse;
				flag = true;
			}
			if (Mathf.Abs(velocity.x) < minHorizontalSpeed && Mathf.Abs(velocity.x) > 0.01f)
			{
				float num2 = ((velocity.x >= 0f) ? 1f : (-1f));
				zero.x = num2 * horizontalImpulse;
				flag = true;
			}
			if (flag)
			{
				rb.AddForce(zero, ForceMode2D.Impulse);
			}
			if (rb.velocity.magnitude <= 0f)
			{
				base.transform.position = rufus.transform.position;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (Mathf.Abs(rb.velocity.y) < minVerticalSpeed)
		{
			float num = ((collision.GetContact(0).normal.y >= 0f) ? (-1f) : 1f);
			Vector2 force = new Vector2(0f, num * verticalImpulse);
			rb.AddForce(force, ForceMode2D.Impulse);
		}
		if (Mathf.Abs(rb.velocity.x) < minHorizontalSpeed)
		{
			float num2 = ((collision.GetContact(0).normal.x >= 0f) ? (-1f) : 1f);
			Vector2 force2 = new Vector2(num2 * horizontalImpulse, 0f);
			rb.AddForce(force2, ForceMode2D.Impulse);
		}
	}
}
