using FMODUnity;
using UnityEngine;

public class RufusFollowArm : StateMachineBehaviour
{
	private RufusArm arm;

	private Rufus rufus;

	private GameObject player;

	private int count;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		rufus = animator.GetComponent<Rufus>();
		arm = Object.FindObjectOfType<RufusArm>();
		Vector3 position = new Vector3(arm.gameObject.transform.position.x, rufus.startingPos.y, rufus.startingPos.z);
		animator.gameObject.transform.position = rufus.ClampPositionToBounds(position);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Time.timeScale == 0f || !(arm != null))
		{
			return;
		}
		Vector3 position = new Vector3(arm.gameObject.transform.position.x, rufus.startingPos.y, rufus.startingPos.z);
		position = rufus.ClampPositionToBounds(position);
		float num = Vector2.Distance(arm.gameObject.transform.position, animator.gameObject.transform.position);
		Vector3 normalized = (animator.gameObject.transform.position - arm.gameObject.transform.position).normalized;
		float num2 = Mathf.Max(0f, Vector3.Dot(arm.currentVel, -normalized));
		float num3 = 5f;
		float num4 = 2f;
		float num5 = 20f;
		float num6 = Mathf.Clamp01(num / num3);
		float num7 = 1f - num6;
		float b = num4 + (num5 - num4) * num7;
		float num8 = Mathf.Max(num2 * 1.5f + 5f, b);
		animator.gameObject.transform.position = Vector3.Lerp(animator.gameObject.transform.position, position, Time.deltaTime * num8);
		if (num <= 1f)
		{
			if (count < 8)
			{
				player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
				Vector3 vector = player.transform.position - arm.gameObject.transform.position;
				arm.gameObject.GetComponent<Rigidbody2D>().AddForce(-arm.GetComponent<Rigidbody2D>().velocity, ForceMode2D.Impulse);
				arm.direction = vector.normalized;
				arm.GetComponent<Rigidbody2D>().AddForce(vector.normalized * 10f, ForceMode2D.Impulse);
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ArmWoosh", animator.gameObject.transform.position);
				animator.SetTrigger("HitArm");
				arm.hitTimer = 0f;
				count++;
			}
			else
			{
				count = 0;
				Object.Destroy(arm.gameObject);
				animator.SetBool("FollowingArm", value: false);
				animator.SetBool("Walking", value: true);
			}
		}
		if (arm.currentVel.x > 0f && animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
		else if (arm.currentVel.x < 0f && !animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			animator.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
