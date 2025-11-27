using System.Collections;
using FMODUnity;
using UnityEngine;

public class CryliaTrap : MonoBehaviour
{
	private GameObject player;

	private float timer;

	private float percent;

	private Vector3 startingPos;

	private Vector3 fallPos;

	private Crylia crylia;

	private bool audioPlayed;

	private bool fallingAudioPlayed;

	private Animator anim;

	private float deathTimer = 18f;

	private bool killed;

	private bool preventTimeout;

	private bool playerTrapped;

	private Rigidbody2D playerRb;

	private void Start()
	{
		anim = GetComponent<Animator>();
		audioPlayed = false;
		crylia = Object.FindObjectOfType<Crylia>();
		fallPos = base.gameObject.transform.position + new Vector3(0f, -5f, 0f);
		startingPos = base.gameObject.transform.position;
		timer = 0f;
		percent = 0f;
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		Collider2D component = GetComponent<Collider2D>();
		if (!(component != null))
		{
			return;
		}
		CryliaEquiment[] array = Object.FindObjectsOfType<CryliaEquiment>();
		foreach (CryliaEquiment cryliaEquiment in array)
		{
			if (cryliaEquiment.GetComponent<Collider2D>() != null)
			{
				Physics2D.IgnoreCollision(component, cryliaEquiment.GetComponent<Collider2D>());
			}
		}
	}

	private void Update()
	{
		if (crylia == null)
		{
			Object.Destroy(base.gameObject);
		}
		timer += Time.deltaTime;
		percent = timer / 0.3f;
		if (percent <= 1f)
		{
			if (!fallingAudioPlayed)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Falling", base.transform.position);
				fallingAudioPlayed = true;
			}
			base.transform.position = Vector3.Lerp(startingPos, fallPos, percent);
		}
		else if (!audioPlayed)
		{
			Screenshake.Instance.AddTrauma(0.5f);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/TrapHittingFloor", base.transform.position);
			audioPlayed = true;
		}
		if (!playerTrapped && percent > 1f && !preventTimeout && TryGetCollidingPlayer(out var collidingPlayer, out var collidingPlayerAlreadyTrapped))
		{
			if (collidingPlayerAlreadyTrapped)
			{
				KillIdleTrap();
			}
			else if (CryliaTrapCoordinator.TryBeginTrap(collidingPlayer, base.gameObject))
			{
				player = collidingPlayer;
				playerTrapped = true;
				preventTimeout = true;
				playerRb = player.GetComponent<Rigidbody2D>();
				MaintainTrappedPlayerPosition();
				player.GetComponent<Health>().Damage(20f);
				player.GetComponent<Movement>().Stun(3.5f);
				player.transform.position = base.gameObject.transform.position;
				StartCoroutine(DelayedDestroy(3.5f));
			}
		}
		deathTimer -= Time.deltaTime;
		if (deathTimer <= 0f && !killed && !preventTimeout)
		{
			killed = true;
			anim.SetTrigger("Death");
		}
	}

	private void FixedUpdate()
	{
		if (playerTrapped)
		{
			MaintainTrappedPlayerPosition();
		}
	}

	private IEnumerator DelayedDestroy(float delay)
	{
		yield return new WaitForSeconds(delay);
		ReleasePlayer();
		anim.SetTrigger("Death");
	}

	private void DestroyObject()
	{
		ReleasePlayer();
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		ReleasePlayer();
	}

	private void ReleasePlayer()
	{
		if (playerTrapped)
		{
			CryliaTrapCoordinator.EndTrap(player, base.gameObject);
			playerTrapped = false;
		}
	}

	private void MaintainTrappedPlayerPosition()
	{
		if (!(player == null))
		{
			if (playerRb == null)
			{
				playerRb = player.GetComponent<Rigidbody2D>();
			}
			Vector3 position = base.gameObject.transform.position;
			Vector2 position2 = new Vector2(position.x, position.y);
			if (playerRb != null)
			{
				playerRb.MovePosition(position2);
				playerRb.velocity = Vector2.zero;
			}
			else
			{
				player.transform.position = position;
			}
		}
	}

	private void KillIdleTrap()
	{
		if (!killed)
		{
			killed = true;
			anim.SetTrigger("Death");
		}
	}

	public void ForceDestroyFromCoordinator()
	{
		if (!killed)
		{
			preventTimeout = true;
			killed = true;
			ReleasePlayer();
			anim.SetTrigger("Death");
		}
	}

	private bool TryGetCollidingPlayer(out GameObject collidingPlayer, out bool collidingPlayerAlreadyTrapped)
	{
		collidingPlayer = null;
		collidingPlayerAlreadyTrapped = false;
		GameObject[] players = PlayerManager.players;
		if (players != null && players.Length != 0)
		{
			GameObject[] array = players;
			foreach (GameObject gameObject in array)
			{
				if (!(gameObject == null) && Vector2.Distance(gameObject.transform.position, base.gameObject.transform.position) <= 0.45f)
				{
					collidingPlayer = gameObject;
					collidingPlayerAlreadyTrapped = CryliaTrapCoordinator.PlayerHasState(gameObject, CryliaHoldSource.Trap);
					return true;
				}
			}
		}
		if (player != null && Vector2.Distance(player.transform.position, base.gameObject.transform.position) <= 0.45f)
		{
			collidingPlayer = player;
			collidingPlayerAlreadyTrapped = CryliaTrapCoordinator.PlayerHasState(player, CryliaHoldSource.Trap);
			return true;
		}
		return false;
	}
}
