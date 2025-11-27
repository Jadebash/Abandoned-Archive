using UnityEngine;

public class LemuresMinionIdle : StateMachineBehaviour
{
	private float cooldownTime;

	private float timer;

	private int lastRandom = -1;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		cooldownTime = Random.Range(0.3f, 0.6f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime * animator.GetFloat("TimeMultiplier");
		if (timer >= cooldownTime)
		{
			int num = Random.Range(0, 3);
			switch (num)
			{
			case 0:
				animator.SetTrigger("Dash");
				break;
			case 1:
				animator.SetTrigger("Beam");
				break;
			case 2:
				animator.SetTrigger("AstralDash");
				break;
			}
			lastRandom = num;
			timer -= cooldownTime;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("Dash");
		animator.ResetTrigger("Beam");
		animator.ResetTrigger("AstralDash");
	}
}
