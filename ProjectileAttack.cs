using UnityEngine;

public class ProjectileAttack : StateMachineBehaviour
{
	private EnemyProjectile enemy;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		enemy = animator.GetComponentInParent<EnemyProjectile>();
		timer = 1f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (!(timer > enemy.timeBetweenAttacks))
		{
			return;
		}
		if (enemy.rhythm <= 0)
		{
			timer = enemy.timeBetweenAttacks / enemy.rhythmResetAmount;
			enemy.rhythm++;
		}
		else if (enemy.rhythm == 1)
		{
			if (Random.Range(0f, 100f) < enemy.rhythmResetChance)
			{
				timer = 0f;
				enemy.rhythm = 0;
			}
			else
			{
				timer = 0f;
				enemy.rhythm = -1;
			}
		}
		enemy.Shoot();
	}
}
