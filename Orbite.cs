using UnityEngine;

public class Orbite : MonoBehaviour
{
	private GameObject player;

	private Vector3 direction;

	public float speed;

	[HideInInspector]
	public GameObject oblome;

	private Health health;

	private bool dead;

	private SpriteRenderer sprite;

	private int colour;

	private float timer;

	private Orbite[] otherOrbites;

	private void Start()
	{
		player = PlayerManager.ClosestPlayer(base.transform.position);
		health = GetComponent<Health>();
		sprite = base.transform.GetChild(0).GetComponent<SpriteRenderer>();
		colour = Random.Range(0, 5);
		if (colour == 0)
		{
			sprite.color = Color.red;
		}
		else if (colour == 1)
		{
			sprite.color = Color.blue;
		}
		else if (colour == 2)
		{
			sprite.color = Color.green;
		}
		else if (colour == 3)
		{
			sprite.color = Color.yellow;
		}
		else if (colour == 4)
		{
			sprite.color = Color.cyan;
		}
		otherOrbites = Object.FindObjectsOfType<Orbite>();
	}

	private void Update()
	{
		if (health.health > 95f)
		{
			direction = (player.transform.position - base.transform.position).normalized;
			Orbite[] array = otherOrbites;
			foreach (Orbite orbite in array)
			{
				if (orbite != null && orbite.gameObject != base.gameObject && Vector2.Distance(base.transform.position, orbite.gameObject.transform.position) < 1f)
				{
					direction = -(orbite.gameObject.transform.position - base.transform.position).normalized;
				}
			}
		}
		else
		{
			direction = (oblome.transform.position - base.transform.position).normalized;
			if (Vector3.Distance(base.transform.position, oblome.transform.position) < 0.25f)
			{
				Object.Destroy(base.gameObject);
			}
		}
		if (health.health <= 95f && !dead)
		{
			dead = true;
			speed *= 3.5f;
			oblome.GetComponent<Health>().health += 20f;
			GetComponent<Animator>().SetTrigger("Die");
		}
		timer += Time.deltaTime;
		if (timer >= 7.5f)
		{
			GetComponent<Animator>().SetTrigger("Die");
		}
		if (oblome != null && oblome.GetComponent<Health>().health <= 0f)
		{
			GetComponent<Animator>().SetTrigger("Die");
		}
		base.transform.position += direction * Time.deltaTime * speed;
	}

	public void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
