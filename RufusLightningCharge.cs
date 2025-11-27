using FMODUnity;
using UnityEngine;

public class RufusLightningCharge : StateMachineBehaviour
{
	private GameObject player;

	private Rigidbody2D rb;

	private Vector2 dir;

	private float chargeTimer;

	public float speed;

	private bool charging;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		charging = false;
		chargeTimer = 2f;
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		rb = animator.GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.mass = 10000f;
		dir = player.transform.position - animator.gameObject.transform.position;
		rb.AddForce(dir.normalized * 1.5f * rb.mass, ForceMode2D.Impulse);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		chargeTimer -= Time.deltaTime;
		if (rb.velocity.magnitude >= 5f && !charging)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusCharge", animator.gameObject.transform.position);
			charging = true;
		}
		if (charging && rb.velocity.magnitude < 5f)
		{
			FinishCharge(animator);
		}
		if (chargeTimer > 0f)
		{
			rb.AddForce(dir * speed * rb.mass * Time.deltaTime);
			if (Vector2.Distance(player.transform.position, animator.gameObject.transform.position) <= 1.5f)
			{
				Screenshake.Instance.AddTrauma(0.5f);
				chargeTimer = 0f;
				rb.velocity = new Vector2(0f, 0f);
				animator.SetTrigger("StrikePlayer");
			}
			if (animator.gameObject.transform.position.x > player.transform.position.x && !animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
			{
				animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
			}
		}
		else
		{
			FinishCharge(animator);
		}
	}

	private void FinishCharge(Animator animator)
	{
		Screenshake.Instance.AddTrauma(0.5f);
		rb.velocity = Vector2.zero;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusHitWall", animator.gameObject.transform.position);
		animator.SetTrigger("Discharge");
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rb.velocity = new Vector2(0f, 0f);
		animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
	}
}
