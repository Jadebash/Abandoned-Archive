using UnityEngine;

public class RufusHitArm : StateMachineBehaviour
{
	private RufusArm arm;

	private Rufus rufus;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rufus = animator.GetComponent<Rufus>();
		arm = Object.FindObjectOfType<RufusArm>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Vector3 position = new Vector3(arm.gameObject.transform.position.x, rufus.startingPos.y, rufus.startingPos.z);
		animator.gameObject.transform.position = rufus.ClampPositionToBounds(position);
	}
}
