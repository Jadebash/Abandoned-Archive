using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GreenFire : MonoBehaviour
{
	private bool extinguished;

	private void Awake()
	{
		CircleCollider2D component = GetComponent<CircleCollider2D>();
		component.isTrigger = true;
		if (component.radius < 0.1f)
		{
			component.radius = 0.75f;
		}
	}

	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		HandleCollision(other);
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		HandleCollision(other);
	}

	private void HandleCollision(Collider2D other)
	{
		if (extinguished || !other.CompareTag("Player"))
		{
			return;
		}
		Movement component = other.GetComponent<Movement>();
		if (component != null && component.rolling)
		{
			Extinguish();
			return;
		}
		Health component2 = other.GetComponent<Health>();
		if (component2 != null)
		{
			component2.Damage(20f);
			Extinguish();
		}
	}

	private void Extinguish()
	{
		if (!extinguished)
		{
			extinguished = true;
			GetComponent<Animator>().SetTrigger("FadeOut");
			GetComponent<Collider2D>().enabled = false;
		}
	}
}
