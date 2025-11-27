using UnityEngine;

public class RodhSpeaking : StateMachineBehaviour
{
	private int randMessage;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		randMessage = Random.Range(0, animator.gameObject.GetComponent<Rodh>().fightMessages.Length);
		DialogueSystem.Instance.StartDialogue(animator.gameObject.GetComponent<Rodh>().fightMessages[randMessage]);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!DialogueSystem.Instance.inDialogue)
		{
			animator.SetBool("Speaking", value: false);
		}
	}
}
