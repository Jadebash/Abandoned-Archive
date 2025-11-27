using FMODUnity;
using UnityEngine;

public class OblomeLemuresIdle : StateMachineBehaviour
{
	private float cooldownTime;

	private float timer;

	private static float attackAmount;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		cooldownTime = 0.5f;
		timer = 0f;
		if (attackAmount >= 3f)
		{
			attackAmount = 0f;
			animator.SetTrigger("TransformBack");
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/OblomeTransform");
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer >= cooldownTime)
		{
			switch (Random.Range(0, 2))
			{
			case 0:
				attackAmount += 1f;
				animator.SetTrigger("LemuresRed");
				break;
			case 1:
				attackAmount += 1f;
				animator.SetTrigger("LemuresTriple");
				break;
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer = 0f;
	}
}
