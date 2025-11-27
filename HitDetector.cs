using FMODUnity;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
	public enum Sides
	{
		LEFT = 0,
		RIGHT = 1,
		UP = 2,
		DOWN = 3
	}

	public Sides side;

	public GameObject arm;

	private Rigidbody2D rb;

	private GameObject rufus;

	private void Start()
	{
		rufus = Object.FindObjectOfType<Rufus>().gameObject;
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), rufus.GetComponent<Collider2D>());
		rb = arm.GetComponent<Rigidbody2D>();
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), PlayerManager.ClosestPlayer(base.gameObject.transform.position).GetComponent<Collider2D>());
	}

	private void OnCollisionEnter2D()
	{
		Screenshake.Instance.AddTrauma(0.7f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ArmHittingWall", base.transform.position);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ArmWoosh", base.transform.position);
		if (side == Sides.LEFT || side == Sides.RIGHT)
		{
			float x = arm.GetComponent<RufusArm>().currentVel.x;
			float y = arm.GetComponent<RufusArm>().currentVel.y;
			rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
			arm.GetComponent<RufusArm>().direction = new Vector3(0f - x, y, arm.GetComponent<RufusArm>().direction.z).normalized;
			rb.AddForce(arm.GetComponent<RufusArm>().direction * 10f, ForceMode2D.Impulse);
		}
		else
		{
			float x = arm.GetComponent<RufusArm>().currentVel.x;
			float y = arm.GetComponent<RufusArm>().currentVel.y;
			rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
			arm.GetComponent<RufusArm>().direction = new Vector3(x, 0f - y, arm.GetComponent<RufusArm>().direction.z).normalized;
			rb.AddForce(arm.GetComponent<RufusArm>().direction * 10f, ForceMode2D.Impulse);
		}
	}
}
