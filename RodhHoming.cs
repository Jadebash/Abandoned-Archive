using FMODUnity;
using UnityEngine;

public class RodhHoming : StateMachineBehaviour
{
	private GameObject player;

	private Vector2 dir;

	private Vector2 distance;

	private Rigidbody2D rb;

	public float speed;

	public int maxCharges = 6;

	private int chargeCount;

	private bool isCharging;

	private float chargeTimer;

	public GameObject explosionParticles;

	private bool isDetonating;

	private Rodh rodh;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		rb = animator.GetComponent<Rigidbody2D>();
		chargeCount = 0;
		isCharging = true;
		chargeTimer = 0f;
		rodh = animator.GetComponent<Rodh>();
		if (rodh != null && rodh.roomAnim != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = rodh.roomAnim.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName("EmeraldCircle") || currentAnimatorStateInfo.IsTag("EmeraldCircle"))
			{
				return;
			}
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/FlaskWoosh");
		rb.bodyType = RigidbodyType2D.Dynamic;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			return;
		}
		distance = player.transform.position - animator.gameObject.transform.position;
		dir = distance.normalized;
		if (distance.magnitude < 1f)
		{
			if (!isDetonating)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RodhExplosion");
				Object.Instantiate(explosionParticles, animator.gameObject.transform.position, Quaternion.identity);
				isDetonating = true;
			}
		}
		else if (isDetonating)
		{
			isDetonating = false;
		}
		if (rodh != null && rodh.roomAnim != null)
		{
			AnimatorStateInfo currentAnimatorStateInfo = rodh.roomAnim.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName("EmeraldCircle") || currentAnimatorStateInfo.IsTag("EmeraldCircle") || currentAnimatorStateInfo.IsName("+") || currentAnimatorStateInfo.IsTag("+"))
			{
				animator.SetBool("Homing", value: false);
				animator.SetBool("MovingBack", value: true);
				return;
			}
		}
		if (isCharging)
		{
			chargeTimer += Time.deltaTime;
			if (chargeTimer >= 1.5f)
			{
				isCharging = false;
				chargeTimer = 0f;
				chargeCount++;
				rb.velocity = Vector2.zero;
				if (chargeCount >= maxCharges)
				{
					animator.SetBool("Homing", value: false);
					animator.SetBool("MovingBack", value: true);
				}
			}
		}
		else
		{
			chargeTimer += Time.deltaTime;
			if (chargeTimer >= 0.1f && chargeCount < maxCharges)
			{
				isCharging = true;
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/FlaskWoosh");
				chargeTimer = 0f;
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rb.velocity = Vector2.zero;
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!(player == null) && isCharging && Time.timeScale != 0f)
		{
			distance = player.transform.position - animator.gameObject.transform.position;
			dir = distance.normalized;
			rb.AddForce(dir * speed * Time.fixedDeltaTime);
		}
	}
}
