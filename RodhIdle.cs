using FMODUnity;
using UnityEngine;

public class RodhIdle : StateMachineBehaviour
{
	private float timer;

	private float attackTimer;

	private float speechCooldown;

	private int lastAttack = -1;

	private bool justTransitioned;

	private Rodh boss;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		attackTimer = 1.5f;
		animator.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		animator.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		boss = animator.gameObject.GetComponent<Rodh>();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if ((boss.currentRoom == Rodh.RoomState.Square && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) || (boss.currentRoom == Rodh.RoomState.Diamond && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle")) || (boss.currentRoom == Rodh.RoomState.Triangle && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("TriangleIdle")))
		{
			timer += Time.deltaTime;
		}
		speechCooldown -= Time.deltaTime;
		if (!(timer >= attackTimer))
		{
			return;
		}
		if (boss.currentRoom == Rodh.RoomState.Square)
		{
			int num = Random.Range(0, 7);
			int num2 = 0;
			do
			{
				num = Random.Range(0, 7);
				num2++;
			}
			while (num == lastAttack || (justTransitioned && (num == 5 || num == 6 || num == 1)));
			lastAttack = num;
			if (num == 0 && !animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Spear"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("Spear");
				timer = 0f;
				justTransitioned = false;
			}
			else if (num == 1 && speechCooldown <= 0f)
			{
				boss.startSpeech();
				timer = 0f;
				speechCooldown = 10f;
				justTransitioned = false;
			}
			else if (num == 1 && speechCooldown > 0f)
			{
				timer = attackTimer;
				justTransitioned = false;
			}
			else if (num == 2 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("X");
				timer = 0f;
				justTransitioned = false;
			}
			else if (num == 3 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("Spikes");
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomSliding");
				timer = 0f;
				justTransitioned = false;
			}
			else if (num == 4 && !animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Sinking"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetBool("Sinking", value: true);
				timer = 0f;
				justTransitioned = false;
			}
			else if (num == 5 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("SquareToDiamond");
				boss.currentRoom = Rodh.RoomState.Diamond;
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomTransform");
				timer = 0f;
				justTransitioned = true;
			}
			else if (num == 6 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				boss.currentRoom = Rodh.RoomState.Triangle;
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("SquareToTriangle");
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomTransform");
				Debug.Log(boss.currentRoom);
				timer = 0f;
				justTransitioned = true;
			}
			if (Random.Range(0, 3) == 1)
			{
				animator.SetBool("Homing", value: true);
			}
		}
		else if (boss.currentRoom == Rodh.RoomState.Diamond)
		{
			int num3 = 0;
			int num4;
			do
			{
				num4 = Random.Range(0, 7);
				num3++;
			}
			while (num4 == lastAttack || (justTransitioned && (num4 == 4 || num4 == 5 || num4 == 6)));
			lastAttack = num4;
			if (num4 == 0 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("+");
				Object.Instantiate(boss.greenLaser, boss.room.transform.position, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
				timer = 0f;
				justTransitioned = false;
			}
			else if (num4 == 1 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("DiamondCircle");
				GameObject gameObject = PlayerManager.ClosestPlayer(boss.transform.position);
				gameObject.GetComponent<Movement>().Stun(2f);
				boss.SetBubblesMovement(canMove: false);
				Object.Instantiate(boss.emeraldCircle, gameObject.transform.position, Quaternion.identity);
				if (boss.GetComponent<Health>().health <= boss.GetComponent<Health>().maxHealth / 2f)
				{
					Object.Instantiate(boss.largeEmeraldCircle, gameObject.transform.position, Quaternion.identity);
				}
				boss.StartCoroutine(boss.EnableBubbleMovementAfterDelay(3f));
				timer = 0f;
				justTransitioned = false;
			}
			else if (num4 == 2 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("Bubbles");
				animator.gameObject.GetComponent<Rodh>().GreenBubbles();
				timer = 0f;
				justTransitioned = false;
			}
			else if (num4 == 3 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetBool("OnFire", value: true);
				timer = 0f;
				justTransitioned = false;
			}
			else if (num4 == 4 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("DiamondToSquare");
				boss.currentRoom = Rodh.RoomState.Square;
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomTransform");
				timer = 0f;
				justTransitioned = true;
			}
			else if (num4 == 5 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("DiamondIdle"))
			{
				boss.currentRoom = Rodh.RoomState.Triangle;
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("DiamondToTriangle");
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomTransform");
				timer = 0f;
				justTransitioned = true;
			}
			else if (num4 == 6 && speechCooldown <= 0f)
			{
				boss.startSpeech();
				speechCooldown = 10f;
				timer = 0f;
				justTransitioned = false;
			}
			else if (num4 == 6 && speechCooldown > 0f)
			{
				timer = attackTimer;
				justTransitioned = false;
			}
			if (Random.Range(0, 3) == 1)
			{
				animator.SetBool("Homing", value: true);
			}
		}
		else if (boss.currentRoom == Rodh.RoomState.Triangle)
		{
			int num5 = 0;
			int num6;
			do
			{
				num6 = Random.Range(0, 5);
				num5++;
			}
			while (num6 == lastAttack || (justTransitioned && (num6 == 0 || num6 == 1 || num6 == 4)));
			lastAttack = num6;
			if (num6 == 0 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("TriangleIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("TriangleToSquare");
				boss.currentRoom = Rodh.RoomState.Square;
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomTransform");
				timer = 0f;
				justTransitioned = true;
			}
			else if (num6 == 1 && animator.gameObject.GetComponent<Rodh>().roomAnim.GetCurrentAnimatorStateInfo(0).IsName("TriangleIdle"))
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("TriangleToDiamond");
				boss.currentRoom = Rodh.RoomState.Diamond;
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RoomTransform");
				timer = 0f;
				justTransitioned = true;
			}
			else if (num6 == 2)
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetBool("TriangleGlow", value: true);
				timer = 0f;
				justTransitioned = false;
			}
			else if (num6 == 3)
			{
				animator.gameObject.GetComponent<Rodh>().roomAnim.SetTrigger("TriangleWater");
				timer = 0f;
				justTransitioned = false;
			}
			else if (num6 == 4 && speechCooldown <= 0f)
			{
				boss.startSpeech();
				speechCooldown = 10f;
				timer = 0f;
				justTransitioned = false;
			}
			else if (num6 == 4 && speechCooldown > 0f)
			{
				timer = attackTimer;
				justTransitioned = false;
			}
			if (Random.Range(0, 3) == 1)
			{
				animator.SetBool("Homing", value: true);
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
