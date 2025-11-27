using FMODUnity;
using UnityEngine;

public class LemuresFlameBalls : MonoBehaviour
{
	private SpriteRenderer sr;

	private bool disabling;

	private float floatingTimer;

	private float forceTimer;

	private float deathTimer;

	private GameObject player;

	private Rigidbody2D rb;

	private bool sfx;

	public LayerMask collisionLayers;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		floatingTimer += Time.deltaTime;
		deathTimer += Time.deltaTime;
		if (floatingTimer < 3f)
		{
			base.transform.position += base.transform.up * Time.deltaTime;
		}
		else
		{
			forceTimer += Time.deltaTime;
			if (forceTimer < 0.5f)
			{
				rb.AddForce(new Vector3(player.transform.position.x - base.gameObject.transform.position.x, player.transform.position.y - base.gameObject.transform.position.y, 0f) * 800f * Time.deltaTime);
				if (!sfx)
				{
					RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Rock_Woosh", base.gameObject.transform.position);
					sfx = true;
				}
			}
		}
		if (forceTimer >= 1f)
		{
			floatingTimer = 0f;
			forceTimer = 0f;
			deathTimer = 0f;
			sfx = false;
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (collisionLayers.value == (collisionLayers.value | (1 << col.gameObject.layer)))
		{
			floatingTimer = 0f;
			forceTimer = 0f;
			deathTimer = 0f;
			sfx = false;
			base.gameObject.SetActive(value: false);
		}
	}
}
