using UnityEngine;

public class RufusThrowArm : StateMachineBehaviour
{
	private RufusArm arm;

	private Rufus rufus;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rufus = animator.GetComponent<Rufus>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		arm = Object.FindObjectOfType<RufusArm>();
		if (arm != null)
		{
			Vector3 b = new Vector3(arm.gameObject.transform.position.x, rufus.startingPos.y, rufus.startingPos.z);
			float num = Vector2.Distance(arm.gameObject.transform.position, animator.gameObject.transform.position);
			Vector3 normalized = (animator.gameObject.transform.position - arm.gameObject.transform.position).normalized;
			float num2 = Mathf.Max(0f, Vector3.Dot(arm.currentVel, -normalized));
			float num3 = 5f;
			float num4 = 2f;
			float num5 = 20f;
			float num6 = Mathf.Clamp01(num / num3);
			float num7 = 1f - num6;
			float b2 = num4 + (num5 - num4) * num7;
			float num8 = Mathf.Max(num2 * 1.5f + 5f, b2);
			animator.gameObject.transform.position = Vector3.Lerp(animator.gameObject.transform.position, b, Time.deltaTime * num8);
		}
	}
}
