using UnityEngine;

public class LemuresMinionIntro : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool("Intro", value: false);
	}
}
