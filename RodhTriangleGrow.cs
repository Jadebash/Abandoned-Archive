using UnityEngine;

public class RodhTriangleGrow : StateMachineBehaviour
{
	private float timer = 12f;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Debug.Log("Triangle Growing");
		timer = 12f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			animator.SetBool("TriangleGrowing", value: false);
		}
	}
}
