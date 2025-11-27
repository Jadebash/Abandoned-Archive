using FMODUnity;
using UnityEngine;

public class CryliaCharge : StateMachineBehaviour
{
	private Crylia crylia;

	private GameObject player;

	private Rigidbody2D rb;

	private SpriteRenderer spriteRenderer;

	private Vector3 startingDir;

	private Vector2 vel = new Vector3(0f, 0f);

	[HideInInspector]
	public bool charging;

	[HideInInspector]
	public bool holdingPlayer;

	public float chargeForce;

	public float maxVelocity = 25f;

	private Vector3 velocity;

	private float x;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		x = 0f;
		timer = 0f;
		crylia = Object.FindObjectOfType<Crylia>();
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		Physics2D.IgnoreCollision(animator.gameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
		startingDir = player.transform.position - animator.gameObject.transform.position;
		rb = animator.GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.velocity = new Vector2(0f, 0f);
		spriteRenderer = crylia.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Charge", animator.gameObject.transform.position);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (timer >= 3f)
		{
			charging = false;
			animator.SetBool("Walking", value: true);
		}
		else
		{
			if (Time.timeScale == 0f || (Manager.Instance != null && Manager.Instance.isPaused))
			{
				return;
			}
			if (rb.velocity.magnitude >= 6f)
			{
				Screenshake.Instance.AddTrauma(0.5f);
			}
			if (rb.velocity.magnitude > 7f)
			{
				charging = true;
			}
			if (charging && rb.velocity.magnitude <= 7f)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/HitWall", animator.gameObject.transform.position);
				Screenshake.Instance.AddTrauma(0.5f);
				charging = false;
				animator.SetBool("Walking", value: true);
				if (holdingPlayer)
				{
					player.GetComponent<Health>().Damage(20f);
				}
			}
			Vector3 vector = player.transform.position - animator.gameObject.transform.position;
			rb.AddForce(vector.normalized * chargeForce * Time.deltaTime);
			if (rb.velocity.magnitude > maxVelocity)
			{
				rb.velocity = rb.velocity.normalized * maxVelocity;
			}
			if (crylia != null)
			{
				animator.gameObject.transform.position = crylia.ClampPositionToBounds(animator.gameObject.transform.position);
			}
			if (Vector2.Distance(player.transform.position, animator.gameObject.transform.position) <= 1.4f && !holdingPlayer)
			{
				holdingPlayer = true;
				CryliaTrapCoordinator.SetPlayerState(player, CryliaHoldSource.Charge, active: true);
				velocity = rb.velocity;
				x = rb.velocity.x;
			}
			if (holdingPlayer)
			{
				if (CryliaTrapCoordinator.PlayerHasState(player, CryliaHoldSource.Trap))
				{
					ReleaseHeldPlayer();
				}
				else
				{
					rb.AddForce(velocity.normalized * chargeForce * Time.deltaTime);
					if (rb.velocity.magnitude > maxVelocity)
					{
						rb.velocity = rb.velocity.normalized * maxVelocity;
					}
					Vector3 position = ((!(x > 0f)) ? (animator.gameObject.transform.position + new Vector3(-1f, 0f, 0f)) : (animator.gameObject.transform.position + new Vector3(1f, 0f, 0f)));
					player.transform.position = crylia.ClampPositionToBounds(position);
					animator.gameObject.transform.position = crylia.ClampPositionToBounds(animator.gameObject.transform.position);
				}
			}
			if (player.transform.position.x < crylia.gameObject.transform.position.x && !spriteRenderer.flipX)
			{
				spriteRenderer.flipX = true;
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (holdingPlayer)
		{
			player.transform.position = crylia.ClampPositionToBounds(animator.gameObject.transform.position);
		}
		if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
		Physics2D.IgnoreCollision(animator.gameObject.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), ignore: false);
		charging = false;
		ReleaseHeldPlayer();
		animator.ResetTrigger("charge");
	}

	private void ReleaseHeldPlayer()
	{
		if (holdingPlayer)
		{
			CryliaTrapCoordinator.SetPlayerState(player, CryliaHoldSource.Charge, active: false);
			holdingPlayer = false;
		}
	}
}
