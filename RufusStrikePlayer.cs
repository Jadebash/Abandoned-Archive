using UnityEngine;

public class RufusStrikePlayer : StateMachineBehaviour
{
	private GameObject player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		animator.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player.transform.position = animator.gameObject.transform.position + new Vector3(0.5f, 0f, 0f);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
	}
}
