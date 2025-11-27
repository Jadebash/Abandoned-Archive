using UnityEngine;

public class OblomeBossIdle : StateMachineBehaviour
{
	private float cooldownTime;

	private float timer;

	private int lastAttack = -1;

	private OblomeBoss oblome;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		oblome = animator.GetComponent<OblomeBoss>();
		float num = animator.GetFloat("TimeMultiplier");
		if (!oblome.spedUp)
		{
			cooldownTime = 1.8f / num;
		}
		else
		{
			cooldownTime = 0.9f / num;
		}
		timer = 0f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (!(timer >= cooldownTime) || !oblome.AllLasersDestroyed())
		{
			return;
		}
		int num;
		do
		{
			num = Random.Range(0, oblome.numOfAttacks);
		}
		while (num == lastAttack && oblome.numOfAttacks > 1);
		lastAttack = num;
		int num2 = ((oblome.gameObject.GetComponent<Health>().health <= oblome.gameObject.GetComponent<Health>().maxHealth / 2f) ? Random.Range(0, 6) : Random.Range(0, 8));
		if (num == 2)
		{
			num2 = 0;
		}
		if (num2 == 1)
		{
			int num3 = Random.Range(0, 4);
			if (num3 == 0)
			{
				oblome.SpawnEnemies();
			}
			else if (num3 == 1 && !oblome.spedUp)
			{
				oblome.spedUp = true;
			}
			else
			{
				switch (num3)
				{
				case 2:
					oblome.InvertAttack();
					break;
				case 3:
					oblome.DropSpells();
					break;
				}
			}
		}
		switch (num)
		{
		case 0:
			animator.SetTrigger("Orbite");
			oblome.Orbite();
			break;
		case 1:
			animator.SetTrigger("Laser");
			break;
		case 2:
			animator.SetTrigger("Split");
			break;
		case 3:
			animator.SetTrigger("HotCold");
			break;
		case 4:
			animator.SetTrigger("ThrowFlask");
			break;
		case 5:
			animator.SetTrigger("BeginTransform");
			break;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer = 0f;
	}
}
