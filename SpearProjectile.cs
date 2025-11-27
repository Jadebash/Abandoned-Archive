using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
	private Rigidbody2D rb;

	private GameObject player;

	public float speed;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		Vector3 vector = player.transform.position - base.gameObject.transform.position;
		base.transform.up = vector.normalized;
		rb.AddForce(base.transform.up * speed, ForceMode2D.Impulse);
	}

	private void Update()
	{
		if (Time.timeScale != 0f)
		{
			rb.AddForce(base.transform.up * speed * Time.deltaTime);
			if (Vector2.Distance(player.transform.position, base.transform.position) <= 1.75f)
			{
				Screenshake.Instance.AddTrauma(0.3f);
			}
		}
	}
}
