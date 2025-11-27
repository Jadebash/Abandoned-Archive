using UnityEngine;

public class SirVeltrineIdle : StateMachineBehaviour
{
	[SerializeField]
	private AttackOption[] attackOptions = new AttackOption[5]
	{
		new AttackOption
		{
			animationTrigger = "CoinThrow",
			odds = 22
		},
		new AttackOption
		{
			animationTrigger = "FireSwipeLeft",
			odds = 22
		},
		new AttackOption
		{
			animationTrigger = "FireSwipeRight",
			odds = 22
		},
		new AttackOption
		{
			animationTrigger = "TeleportAttack",
			odds = 22
		},
		new AttackOption
		{
			animationTrigger = "DealCards",
			odds = 12
		}
	};

	private float cooldownTime;

	private float timer;

	private int lastAttackIndex = -1;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		cooldownTime = Random.Range(0.7f, 2f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime * animator.GetFloat("TimeMultiplier");
		if (timer >= cooldownTime)
		{
			int num = SelectRandomAttack();
			animator.SetTrigger(attackOptions[num].animationTrigger);
			lastAttackIndex = num;
			timer -= cooldownTime;
		}
		bool flipX = PlayerManager.ClosestPlayer(animator.gameObject.transform.position).transform.position.x < animator.gameObject.transform.position.x;
		animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = flipX;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AttackOption[] array = attackOptions;
		foreach (AttackOption attackOption in array)
		{
			animator.ResetTrigger(attackOption.animationTrigger);
		}
	}

	private int SelectRandomAttack()
	{
		int num = 0;
		AttackOption[] array = attackOptions;
		foreach (AttackOption attackOption in array)
		{
			num += attackOption.odds;
		}
		if (lastAttackIndex >= 0 && lastAttackIndex < attackOptions.Length)
		{
			num -= attackOptions[lastAttackIndex].odds;
		}
		int num2 = Random.Range(0, num);
		int num3 = 0;
		for (int j = 0; j < attackOptions.Length; j++)
		{
			if (j != lastAttackIndex)
			{
				num3 += attackOptions[j].odds;
				if (num2 < num3)
				{
					return j;
				}
			}
		}
		if (lastAttackIndex != 0)
		{
			return 0;
		}
		return 1;
	}
}
