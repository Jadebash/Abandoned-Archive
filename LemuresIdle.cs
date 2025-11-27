using System.Collections.Generic;
using UnityEngine;

public class LemuresIdle : StateMachineBehaviour
{
	private float cooldownTime;

	private float timer;

	private int lastRandom = -1;

	private Health health;

	private Lemures lemures;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		health = animator.GetComponent<Health>();
		cooldownTime = Random.Range(0.3f, 0.6f);
		lemures = animator.GetComponent<Lemures>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime * animator.GetFloat("TimeMultiplier");
		if (!(timer >= cooldownTime))
		{
			return;
		}
		if (lemures.minions.Count == 4 && health.health / health.maxHealth < 0.8f)
		{
			animator.SetTrigger("Reanimate");
		}
		else if (lemures.minions.Count == 3 && health.health / health.maxHealth < 0.6f)
		{
			animator.SetTrigger("Reanimate");
		}
		else if (lemures.minions.Count == 2 && health.health / health.maxHealth < 0.4f)
		{
			animator.SetTrigger("Reanimate");
		}
		else if (lemures.minions.Count == 1 && health.health / health.maxHealth < 0.2f)
		{
			animator.SetTrigger("Reanimate");
		}
		else
		{
			List<int> list = new List<int>();
			for (int i = 0; i < lemures.numOfAttacks; i++)
			{
				if (i != lastRandom)
				{
					list.Add(i);
				}
			}
			if (list.Count == 0)
			{
				for (int j = 0; j < lemures.numOfAttacks; j++)
				{
					list.Add(j);
				}
			}
			int num = (lastRandom = list[Random.Range(0, list.Count)]);
			if (lemures.gameObject.GetComponent<Boss>().onPhaseTwo && !lemures.blackSword)
			{
				animator.SetTrigger("BlackFlameSword");
			}
			if (num == 0)
			{
				animator.SetTrigger("Dash");
			}
			else if (num == 1)
			{
				animator.SetTrigger("Beam");
			}
			else if (num == 2)
			{
				animator.SetTrigger("AstralDash");
			}
			else if (num == 3)
			{
				animator.SetTrigger("MoonAttack");
			}
			else if (num == 4)
			{
				animator.SetTrigger("SpawnRedSpirits");
			}
			else if (num == 5 && lemures.blueSpiritTimer >= 15f)
			{
				animator.SetTrigger("SpawnBlueSpirits");
			}
			else if (num == 6 && lemures.health.health <= 650f)
			{
				animator.SetTrigger("SpawnGreenSpirits");
			}
			else if (num == 7 && lemures.blackSword)
			{
				animator.SetTrigger("SpawnFlameBalls");
			}
			else if (num == 8 && lemures.blackSword)
			{
				animator.SetTrigger("TripleBeam");
			}
		}
		timer -= cooldownTime;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("Dash");
		animator.ResetTrigger("Reanimate");
		animator.ResetTrigger("Beam");
		animator.ResetTrigger("MoonAttack");
		animator.ResetTrigger("AstralDash");
		animator.ResetTrigger("SpawnBlueSpirits");
		animator.ResetTrigger("SpawnFlameBalls");
	}
}
