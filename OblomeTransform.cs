using FMODUnity;
using UnityEngine;

public class OblomeTransform : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		switch (Random.Range(0, 5))
		{
		case 0:
			animator.SetTrigger("TransformRufus");
			break;
		case 1:
			animator.SetTrigger("TransformVeltrine");
			break;
		case 2:
			animator.SetTrigger("TransformHolite");
			break;
		case 3:
			animator.SetTrigger("TransformLemures");
			break;
		case 4:
			animator.SetTrigger("TransformCrylia");
			break;
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/OblomeTransform");
	}
}
