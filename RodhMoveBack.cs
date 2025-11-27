using UnityEngine;

public class RodhMoveBack : StateMachineBehaviour
{
	private Vector3 startPos;

	private Vector3 endPos;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool("MovingBack", value: true);
		startPos = animator.gameObject.transform.position;
		endPos = animator.gameObject.GetComponent<Rodh>().startPos;
		animator.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.gameObject.transform.position = Vector3.MoveTowards(animator.gameObject.transform.position, endPos, 10f * Time.deltaTime);
		if (animator.gameObject.transform.position == endPos)
		{
			animator.SetBool("MovingBack", value: false);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
