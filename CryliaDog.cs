using FMODUnity;
using UnityEngine;

public class CryliaDog : MonoBehaviour
{
	private Rigidbody2D rb;

	private GameObject player;

	public float speed;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
	}

	private void Update()
	{
		if (Time.timeScale != 0f)
		{
			Vector3 vector = player.transform.position - base.transform.position;
			rb.AddForce(vector.normalized * speed);
			if (rb.velocity.magnitude >= 8f)
			{
				rb.AddForce(-rb.velocity);
			}
			if (GetComponent<Health>().health <= 0f)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/DogDeath", base.transform.position);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
