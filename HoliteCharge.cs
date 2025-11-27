using FMODUnity;
using UnityEngine;

public class HoliteCharge : StateMachineBehaviour
{
	private float timer;

	private Holite holite;

	private Rigidbody2D rigidbody;

	private bool doneRun;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer = 0f;
		holite = animator.GetComponent<Holite>();
		holite.TeleportCharge();
		rigidbody = animator.GetComponent<Rigidbody2D>();
		doneRun = false;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/HoliteFootsteps", animator.transform.position);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer > 0.25f && !doneRun)
		{
			doneRun = true;
			holite.TeleportRun();
		}
		if (Vector3.Distance(holite.ClosestPlayer().position, holite.transform.position) < 1.1f)
		{
			rigidbody.velocity = Vector2.zero;
			switch (Random.Range(0, 3))
			{
			case 0:
				animator.SetTrigger("Attack1");
				break;
			case 1:
				animator.SetTrigger("Attack2");
				break;
			case 2:
				animator.SetTrigger("Attack3");
				break;
			}
		}
		else if (timer > 1f)
		{
			if (Random.Range(0f, 100f) < 30f)
			{
				holite.ResetPosition();
				animator.SetBool("Running", value: false);
			}
			else
			{
				holite.TeleportCharge();
			}
			doneRun = false;
			timer = 0f;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("Attack1");
		animator.ResetTrigger("Attack2");
		animator.ResetTrigger("Attack3");
	}
}
