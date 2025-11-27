using UnityEngine;

public class ApparitionIdle : StateMachineBehaviour
{
	private float timer;

	private float cooldownTime;

	private LostApparition lost;

	private GameObject player;

	private int lastAttack = -1;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		timer = 0f;
		cooldownTime = 0.9f;
		lost = animator.GetComponent<LostApparition>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer >= cooldownTime)
		{
			int num;
			do
			{
				num = Random.Range(0, lost.numOfAttacks);
			}
			while (num == lastAttack && lost.numOfAttacks > 1);
			lastAttack = num;
			switch (num)
			{
			case 0:
				animator.SetTrigger("SummonBalls");
				break;
			case 1:
				animator.SetTrigger("SummonWraiths");
				break;
			case 2:
				animator.SetTrigger("SpearAttack");
				break;
			case 3:
				animator.SetBool("CirclingPlayer", value: true);
				break;
			case 4:
				animator.SetTrigger("InvertAttack");
				break;
			}
		}
		if (PlayerManager.ClosestPlayer(animator.gameObject.transform.position).transform.position.x < animator.gameObject.transform.position.x && !animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		else if (PlayerManager.ClosestPlayer(animator.gameObject.transform.position).transform.position.x > animator.gameObject.transform.position.x && animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
