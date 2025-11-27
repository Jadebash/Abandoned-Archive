using UnityEngine;

public class RoomSpearAttack : StateMachineBehaviour
{
	private Rodh rodh;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer = 0f;
		rodh = Object.FindObjectOfType<Rodh>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer >= 0.5f)
		{
			int randSpear = Random.Range(0, rodh.spearGlow.Count);
			timer = 0f;
			rodh.doSpawnSpears(randSpear);
		}
	}
}
