using FMODUnity;
using UnityEngine;

public class LemuresDash : StateMachineBehaviour
{
	private Rigidbody2D rigidbody;

	private Lemures lemures;

	private float timer;

	private Transform target;

	public bool astral;

	private bool astralSePlayed;

	private bool sePlayed;

	private bool sePlayed1;

	private Vector3 dir = Vector3.zero;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rigidbody = animator.GetComponent<Rigidbody2D>();
		lemures = animator.GetComponent<Lemures>();
		timer = 0f;
		target = PlayerManager.ClosestPlayer(lemures.transform.position).transform;
		if (astral)
		{
			lemures.astralForm.position = lemures.transform.position;
		}
		sePlayed = false;
		sePlayed1 = false;
		astralSePlayed = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime * animator.GetFloat("TimeMultiplier");
		if (astral)
		{
			if (timer < 0.41f)
			{
				lemures.astralForm.position = Vector3.Lerp(lemures.transform.position, target.position, timer / 0.41f);
				dir = (target.position - lemures.transform.position).normalized;
			}
			else if (timer < 0.9f)
			{
				Screenshake.Instance.AddTrauma(1f);
				rigidbody.velocity = dir * 12f;
				if (!astralSePlayed)
				{
					RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Rock_Woosh", animator.gameObject.transform.position);
					astralSePlayed = true;
				}
			}
			else
			{
				rigidbody.velocity = Vector2.zero;
			}
		}
		else if (timer < 0.33f)
		{
			Vector2 vector = target.position - lemures.transform.position;
			vector += (((target.position - lemures.transform.position).x >= 0f) ? new Vector2(-0.5f, 0f) : new Vector2(0.5f, 0f));
			rigidbody.velocity = vector * 4f;
			if (!sePlayed)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Rock_Woosh", animator.gameObject.transform.position);
				sePlayed = true;
			}
			Screenshake.Instance.AddTrauma(1f);
		}
		else
		{
			if (!sePlayed1)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/SmallWoosh", animator.gameObject.transform.position);
				sePlayed1 = true;
			}
			rigidbody.velocity = Vector2.zero;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("Idle");
	}
}
