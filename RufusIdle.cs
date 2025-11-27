using System.Collections.Generic;
using UnityEngine;

public class RufusIdle : StateMachineBehaviour
{
	private float cooldownTime;

	private float timer;

	private int lastAttack = -1;

	private bool lastWasMelee;

	private Rufus rufus;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rufus = animator.GetComponent<Rufus>();
		rufus.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		cooldownTime = 1f;
		animator.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer >= cooldownTime)
		{
			if (Vector2.Distance(PlayerManager.ClosestPlayer(animator.gameObject.transform.position).transform.position, animator.gameObject.transform.position) <= 3f && !lastWasMelee)
			{
				animator.SetTrigger("Melee");
				lastWasMelee = true;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < rufus.numOfAttacks; i++)
				{
					if (i != lastAttack)
					{
						list.Add(i);
					}
				}
				if (list.Count == 0)
				{
					for (int j = 0; j < rufus.numOfAttacks; j++)
					{
						list.Add(j);
					}
				}
				int num = (lastAttack = list[Random.Range(0, list.Count)]);
				lastWasMelee = false;
				switch (num)
				{
				case 0:
					animator.SetTrigger("ThrowRods");
					break;
				case 1:
					animator.SetTrigger("JumpAttack");
					break;
				case 2:
					animator.SetTrigger("ThrowArm");
					break;
				case 3:
					animator.SetTrigger("Charging");
					break;
				case 4:
					if (Object.FindObjectOfType<RufusFloorboard>() == null)
					{
						animator.SetTrigger("StompGround");
					}
					break;
				}
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
		timer = 0f;
	}
}
