using FMODUnity;
using UnityEngine;

public class Nails : MonoBehaviour
{
	public float speed;

	private Rigidbody2D rb;

	private float movementTimer;

	private bool moving;

	private void Start()
	{
		Color color = GetComponent<SpriteRenderer>().color;
		color.a = 0f;
		GetComponent<SpriteRenderer>().color = color;
		rb = GetComponent<Rigidbody2D>();
		EnemyNail[] array = Object.FindObjectsOfType<EnemyNail>();
		for (int i = 0; i < array.Length; i++)
		{
			Physics2D.IgnoreCollision(array[i].gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		}
		movementTimer = Random.Range(0.25f, 0.45f);
	}

	private void Update()
	{
		if (Time.timeScale != 0f)
		{
			movementTimer -= Time.deltaTime;
			if (movementTimer <= 0f && !moving)
			{
				Color color = GetComponent<SpriteRenderer>().color;
				color.a = 1f;
				GetComponent<SpriteRenderer>().color = color;
				rb.AddForce(base.transform.up * speed * Time.deltaTime, ForceMode2D.Impulse);
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ThrowNails", base.transform.position);
				moving = true;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		Screenshake.Instance.AddTrauma(0.4f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/NailDestroy", base.transform.position);
		Object.Destroy(base.gameObject);
	}
}
