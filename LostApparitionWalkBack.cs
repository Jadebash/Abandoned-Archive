using UnityEngine;

public class LostApparitionWalkBack : StateMachineBehaviour
{
	private Vector3 targetPos;

	private Vector3 startingPos;

	private float percent;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		startingPos = animator.gameObject.transform.position;
		targetPos = animator.GetComponent<LostApparition>().startingPos;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (percent <= 1f)
		{
			timer += Time.deltaTime;
			percent = timer / 2.5f;
			animator.gameObject.transform.position = Vector2.Lerp(startingPos, targetPos, percent);
		}
		else
		{
			animator.SetBool("Walking", value: false);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		percent = 0f;
		timer = 0f;
	}
}
