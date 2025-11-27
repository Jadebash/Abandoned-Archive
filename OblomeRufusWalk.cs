using UnityEngine;

public class OblomeRufusWalk : StateMachineBehaviour
{
	private Vector3 startingPos;

	private Vector3 targetPos;

	private float percent;

	private float timer;

	private OblomeBoss oblome;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		startingPos = animator.gameObject.transform.position;
		oblome = animator.GetComponent<OblomeBoss>();
		targetPos = oblome.startingPos;
		percent = 0f;
		timer = 0f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		float num = animator.GetFloat("TimeMultiplier");
		float num2 = 1.5f / ((num > 0f) ? num : 1f);
		percent = timer / num2;
		if (percent <= 1f)
		{
			animator.gameObject.transform.position = Vector2.Lerp(startingPos, targetPos, percent);
		}
		else
		{
			animator.SetBool("RufusWalking", value: false);
		}
		if (targetPos.x < animator.gameObject.transform.position.x && !animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
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
