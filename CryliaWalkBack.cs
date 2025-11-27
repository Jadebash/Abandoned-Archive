using UnityEngine;

public class CryliaWalkBack : StateMachineBehaviour
{
	private Crylia crylia;

	private Rigidbody2D rb;

	private SpriteRenderer spriteRenderer;

	private Vector3 startingPos;

	private float percent;

	private float timer;

	private float timeToWalkBack = 1.5f;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		percent = 0f;
		timer = 0f;
		crylia = animator.GetComponent<Crylia>();
		rb = animator.GetComponent<Rigidbody2D>();
		spriteRenderer = animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		startingPos = animator.gameObject.transform.position;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		percent = timer / timeToWalkBack;
		if (percent <= 1f)
		{
			animator.gameObject.transform.position = Vector3.Lerp(startingPos, crylia.startingPos, percent);
		}
		else
		{
			animator.gameObject.transform.position = crylia.startingPos;
			rb.bodyType = RigidbodyType2D.Kinematic;
			animator.SetBool("Walking", value: false);
		}
		if (crylia.startingPos.x < crylia.gameObject.transform.position.x && !spriteRenderer.flipX)
		{
			spriteRenderer.flipX = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}
}
