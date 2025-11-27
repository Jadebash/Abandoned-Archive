using UnityEngine;

public class OblomeCryliaIWalk : StateMachineBehaviour
{
	private OblomeBoss oblomeBoss;

	private Rigidbody2D rb;

	private Vector3 startingPos;

	private float percent;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		percent = 0f;
		timer = 0f;
		oblomeBoss = animator.GetComponent<OblomeBoss>();
		rb = animator.GetComponent<Rigidbody2D>();
		startingPos = animator.gameObject.transform.position;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		float num = animator.GetFloat("TimeMultiplier");
		float num2 = 1.5f / ((num > 0f) ? num : 1f);
		percent = timer / num2;
		if (percent <= 1f)
		{
			animator.gameObject.transform.position = Vector3.Lerp(startingPos, oblomeBoss.startingPos, percent);
		}
		else
		{
			animator.gameObject.transform.position = oblomeBoss.startingPos;
			rb.bodyType = RigidbodyType2D.Kinematic;
			animator.SetBool("CryliaWalking", value: false);
		}
		if (oblomeBoss.startingPos.x < oblomeBoss.gameObject.transform.position.x && !oblomeBoss.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			oblomeBoss.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (oblomeBoss.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			oblomeBoss.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}
}
