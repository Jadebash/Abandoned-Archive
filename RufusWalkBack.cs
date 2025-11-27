using UnityEngine;

public class RufusWalkBack : StateMachineBehaviour
{
	private Vector3 startingPos;

	private Vector3 targetPos;

	private float percent;

	private float timer;

	private Rufus rufus;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		startingPos = animator.gameObject.transform.position;
		rufus = animator.GetComponent<Rufus>();
		targetPos = rufus.startingPos;
		percent = 0f;
		timer = 0f;
		if (targetPos.x > animator.gameObject.transform.position.x)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
		else
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		percent = timer / 1.5f;
		if (percent <= 1f)
		{
			animator.gameObject.transform.position = Vector2.Lerp(startingPos, targetPos, percent);
		}
		else
		{
			animator.SetBool("Walking", value: false);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}
}
