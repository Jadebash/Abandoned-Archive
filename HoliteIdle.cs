using UnityEngine;

public class HoliteIdle : StateMachineBehaviour
{
	private float cooldownTime;

	private float timer;

	private int lastRandom = -1;

	private Health health;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		health = animator.GetComponent<Health>();
		if (health.health / health.maxHealth < 0.25f)
		{
			cooldownTime = Random.Range(0f, 0.6f);
		}
		else
		{
			cooldownTime = Random.Range(0.2f, 1.4f);
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime * animator.GetFloat("TimeMultiplier");
		if (timer >= cooldownTime)
		{
			int num;
			for (num = Random.Range(0, 5); num == lastRandom; num = Random.Range(0, 5))
			{
			}
			switch (num)
			{
			case 0:
				animator.SetTrigger("StartRed");
				break;
			case 1:
				animator.SetTrigger("StartGreen");
				break;
			case 2:
				animator.SetTrigger("StartYellow");
				break;
			case 3:
				animator.SetBool("Running", value: true);
				break;
			case 4:
				animator.SetTrigger("Combo 1");
				break;
			}
			lastRandom = num;
			timer -= cooldownTime;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("StartRed");
		animator.ResetTrigger("StartGreen");
		animator.ResetTrigger("StartYellow");
	}
}
