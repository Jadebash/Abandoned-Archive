using UnityEngine;

public class OblomeEndingSequence : StateMachineBehaviour
{
	public Dialogue oe1;

	private float dialogueTimer = 1f;

	private bool dialogueStarted;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		dialogueTimer -= Time.deltaTime;
		if (dialogueTimer <= 0f && !dialogueStarted)
		{
			dialogueStarted = true;
			DialogueSystem.Instance.StartDialogue(oe1);
		}
		if (!DialogueSystem.Instance.inDialogue && dialogueStarted)
		{
			animator.SetTrigger("Death");
		}
	}
}
