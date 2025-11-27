using UnityEngine;

public class SirVeltrineStartPhaseTwo : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		SirVeltrine component = animator.GetComponent<SirVeltrine>();
		if (component != null)
		{
			component.StartPhaseTwo();
		}
	}
}
