using UnityEngine;

public class OblomeVeltrineTeleport : StateMachineBehaviour
{
	private int numOfTimesToTeleport = 8;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer = 0f;
		numOfTimesToTeleport = Random.Range(5, 8);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime * animator.GetFloat("VeltrineTimeMultiplier");
		if (timer / stateInfo.length >= (float)numOfTimesToTeleport)
		{
			animator.SetTrigger("VeltrineTeleportBack");
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("VeltrineTeleportBack");
	}
}
