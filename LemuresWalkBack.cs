using UnityEngine;

public class LemuresWalkBack : StateMachineBehaviour
{
	private Rigidbody2D rigidbody;

	private Lemures lemures;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rigidbody = animator.GetComponent<Rigidbody2D>();
		lemures = animator.GetComponent<Lemures>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (lemures == null || lemures.health == null || lemures.health.health <= 0f)
		{
			animator.SetTrigger("Idle");
			animator.SetTrigger("Dead");
			return;
		}
		rigidbody.velocity = (lemures.startingPosition - lemures.transform.position).normalized * lemures.walkSpeed;
		if (Vector3.Distance(lemures.transform.position, lemures.startingPosition) < 0.2f)
		{
			lemures.ResetPosition();
			animator.SetTrigger("Idle");
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("Idle");
	}
}
