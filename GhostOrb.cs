using FMODUnity;
using UnityEngine;

public class GhostOrb : MonoBehaviour
{
	private GameObject player;

	private Rigidbody2D rb;

	private float deathTimer = 0.65f;

	public GameObject particles;

	private AttackHelper attackHelper;

	private void Start()
	{
		attackHelper = GetComponent<AttackHelper>();
		attackHelper.FindPlayableSpaceBounds();
		rb = GetComponent<Rigidbody2D>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		float x = Random.Range(-1.5f, 1.5f);
		float y = Random.Range(-1.5f, 1.5f);
		if (attackHelper != null)
		{
			Debug.Log("Clamping position");
			base.transform.position = attackHelper.ClampPositionToBounds(player.transform.position + new Vector3(x, y, 0f));
		}
		else
		{
			base.transform.position = player.transform.position + new Vector3(x, y, 0f);
		}
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		deathTimer -= Time.deltaTime;
		rb.AddForce(base.transform.up * 50f * Time.deltaTime);
		if (deathTimer <= 0f)
		{
			if (Vector2.Distance(base.gameObject.transform.position, player.transform.position) <= 2f)
			{
				player.GetComponent<Health>().Damage(20f);
			}
			Screenshake.Instance.AddTrauma(0.5f);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/OrbExploding", base.transform.position);
			Object.Instantiate(particles, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
	}
}
