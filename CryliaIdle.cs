using UnityEngine;

public class CryliaIdle : StateMachineBehaviour
{
	private float timer;

	private float cooldownTime;

	private Crylia crylia;

	private int lastAttackIndex = -1;

	private GameObject player;

	private SpriteRenderer spriteRenderer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float num = animator.GetFloat("TimeMultiplier");
		cooldownTime = 0.8f / num;
		crylia = animator.GetComponent<Crylia>();
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		spriteRenderer = animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		animator.SetBool("WalkToCage", value: false);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.gameObject.transform.position = crylia.startingPos;
		timer += Time.deltaTime;
		if (timer >= cooldownTime)
		{
			int num;
			do
			{
				num = Random.Range(0, crylia.numOfAttacks);
			}
			while ((num == lastAttackIndex && crylia.numOfAttacks > 1) || num == 9);
			lastAttackIndex = num;
			if (num == 0)
			{
				animator.SetTrigger("throwBlue");
			}
			else if (num == 1)
			{
				animator.SetTrigger("throwRed");
			}
			else if (num == 2)
			{
				animator.SetTrigger("throwBlack");
			}
			else if (num == 3)
			{
				animator.SetTrigger("throwEquipment");
			}
			else if (num == 4)
			{
				animator.SetTrigger("charge");
			}
			else if (num == 5)
			{
				animator.SetTrigger("claw");
			}
			else if (num == 6)
			{
				animator.SetTrigger("throwCage");
			}
			else if (num == 7 && Object.FindObjectOfType<CryliaTrap>() == null)
			{
				animator.SetTrigger("throwTraps");
			}
			else if (num == 8)
			{
				animator.SetTrigger("Pounce");
			}
			else if (num != 9 || !(Object.FindObjectOfType<CryliaKennel>() == null))
			{
				switch (num)
				{
				case 10:
					animator.SetTrigger("ThrowNails");
					break;
				case 11:
					animator.SetTrigger("throwGreen");
					break;
				}
			}
			timer -= cooldownTime;
		}
		if (player != null && spriteRenderer != null)
		{
			if (player.transform.position.x < animator.gameObject.transform.position.x && !spriteRenderer.flipX)
			{
				spriteRenderer.flipX = true;
			}
			else if (player.transform.position.x > animator.gameObject.transform.position.x && spriteRenderer.flipX)
			{
				spriteRenderer.flipX = false;
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("throwBlue");
		animator.ResetTrigger("throwRed");
		animator.ResetTrigger("throwBlack");
		animator.ResetTrigger("throwEquipment");
		animator.ResetTrigger("charge");
	}
}
