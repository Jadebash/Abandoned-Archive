using FMODUnity;
using UnityEngine;

public class LostApparitionInvert : StateMachineBehaviour
{
	private GameObject player;

	private Rigidbody2D rb;

	public float speed;

	private bool hit;

	private float timer = 1.5f;

	public Material playerGlow;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer = 1.5f;
		hit = false;
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		rb = animator.gameObject.GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/GhostWoosh", animator.gameObject.transform.position);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		Vector3 vector = player.transform.position - animator.gameObject.transform.position;
		rb.AddForce(vector.normalized * speed * Time.deltaTime);
		if (Vector2.Distance(player.transform.position, animator.gameObject.transform.position) <= 1f && !hit)
		{
			hit = true;
			player.GetComponent<Movement>().inverted = true;
			rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
			rb.bodyType = RigidbodyType2D.Kinematic;
			animator.SetTrigger("Fade");
			Screenshake.Instance.AddTrauma(0.5f);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/GhostWhisper", animator.gameObject.transform.position);
			player.transform.Find("Model").GetComponent<SpriteRenderer>().material = playerGlow;
		}
		else
		{
			timer -= Time.deltaTime;
			if (timer <= 0f && !hit)
			{
				hit = true;
				rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
				rb.bodyType = RigidbodyType2D.Kinematic;
				timer = 1.5f;
				animator.SetBool("Walking", value: true);
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("InvertAttack");
	}
}
