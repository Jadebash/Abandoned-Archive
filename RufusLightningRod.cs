using FMODUnity;
using UnityEngine;

public class RufusLightningRod : MonoBehaviour
{
	private Rigidbody2D rb;

	private float movementTimer = 0.75f;

	private float speed = 25f;

	private Rufus rufus;

	private OblomeBoss oblome;

	private bool moving;

	private void Start()
	{
		if (Object.FindObjectOfType<Rufus>() != null)
		{
			rufus = Object.FindObjectOfType<Rufus>();
		}
		if (Object.FindObjectOfType<OblomeBoss>() != null)
		{
			oblome = Object.FindObjectOfType<OblomeBoss>();
		}
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
		if (rufus != null)
		{
			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), rufus.gameObject.GetComponent<Collider2D>());
		}
		if (oblome != null)
		{
			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), oblome.gameObject.GetComponent<Collider2D>());
		}
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(-base.transform.up * speed, ForceMode2D.Impulse);
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log("Rod hit");
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RodImpact", base.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
		rb.velocity = new Vector2(0f, 0f);
	}
}
