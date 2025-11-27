using UnityEngine;

public class RodhWater : StateMachineBehaviour
{
	private Rodh boss;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		boss = Object.FindObjectOfType<Rodh>();
		timer = 0f;
		Object.Instantiate(boss.water, boss.roomAnim.gameObject.transform.position, Quaternion.identity).transform.localScale = new Vector3(1.1f, 1.1f, 1f);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer >= 0.5f)
		{
			int randWater = Random.Range(0, boss.waterGlow.Count);
			timer = 0f;
			boss.doSpawnWaterGlow(randWater);
		}
	}
}
