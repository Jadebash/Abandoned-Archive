using UnityEngine;

public class CryliaWalkToCage : StateMachineBehaviour
{
	private GameObject cage;

	private Vector3 startingPos;

	private float percent;

	private float timer;

	private SpriteRenderer spriteRenderer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		percent = 0f;
		timer = 0f;
		cage = GameObject.Find("CryliaCage(Clone)");
		startingPos = animator.gameObject.transform.position;
		spriteRenderer = animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		percent = timer / 2f;
		if (percent <= 1f)
		{
			animator.gameObject.transform.position = Vector3.Lerp(startingPos, cage.transform.position + new Vector3(-1f, 0f, 0f), percent);
		}
		else
		{
			animator.SetTrigger("KickCage");
		}
		if (animator.gameObject.transform.position.x > cage.transform.position.x && !spriteRenderer.flipX)
		{
			spriteRenderer.flipX = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
		if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}
}
