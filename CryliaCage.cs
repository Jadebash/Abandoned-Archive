using FMODUnity;
using UnityEngine;

public class CryliaCage : MonoBehaviour
{
	private GameObject player;

	private float timer;

	private float percent;

	private Vector3 startingPos;

	private Vector3 fallPos;

	private bool playerTrapped;

	private Crylia crylia;

	private bool audioPlayed;

	private bool fallingAudioPlayed;

	private void Start()
	{
		audioPlayed = false;
		crylia = Object.FindObjectOfType<Crylia>();
		fallPos = crylia.ClampPositionToBounds(base.gameObject.transform.position + new Vector3(0f, -5f, 0f));
		startingPos = base.gameObject.transform.position;
		timer = 0f;
		percent = 0f;
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), crylia.gameObject.GetComponent<Collider2D>());
	}

	private void FixedUpdate()
	{
		if (playerTrapped)
		{
			Rigidbody2D component = player.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				component.position = base.gameObject.transform.position;
				component.velocity = Vector2.zero;
			}
			else
			{
				player.transform.position = base.gameObject.transform.position;
			}
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		percent = timer / 0.35f;
		if (percent <= 1f && !playerTrapped)
		{
			if (!fallingAudioPlayed)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Falling", base.transform.position);
				fallingAudioPlayed = true;
			}
			base.transform.position = Vector3.Lerp(startingPos, fallPos, percent);
			if (player != null && Vector2.Distance(player.transform.position, base.gameObject.transform.position) <= 0.9f && !player.GetComponent<Movement>().rolling)
			{
				SetPlayerTrappedState(value: true);
			}
		}
		else if (!audioPlayed)
		{
			Screenshake.Instance.AddTrauma(0.5f);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/CageHittingFloor", base.transform.position);
			audioPlayed = true;
		}
		if (percent >= 1.5f && !playerTrapped)
		{
			Object.Destroy(base.gameObject);
		}
		if (playerTrapped)
		{
			base.gameObject.transform.position = crylia.ClampPositionToBounds(base.gameObject.transform.position);
			crylia.setPlayerTrapped();
		}
		Rigidbody2D component = GetComponent<Rigidbody2D>();
		if (component != null && component.bodyType == RigidbodyType2D.Dynamic)
		{
			base.gameObject.transform.position = crylia.ClampPositionToBounds(base.gameObject.transform.position);
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (crylia.kickedPlayer)
		{
			Screenshake.Instance.AddTrauma(0.5f);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/CageBreaking", base.transform.position);
			player.GetComponent<Health>().Damage(20f, null, ignoreInvincibility: true);
			Rigidbody2D component = player.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				component.position = crylia.ClampPositionToBounds(base.gameObject.transform.position);
			}
			else
			{
				player.transform.position = crylia.ClampPositionToBounds(base.gameObject.transform.position);
			}
			Object.Destroy(base.gameObject);
			crylia.cageMoving = false;
			crylia.kickedPlayer = false;
		}
	}

	private void OnDestroy()
	{
		SetPlayerTrappedState(value: false);
	}

	private void SetPlayerTrappedState(bool value)
	{
		if (player == null)
		{
			playerTrapped = value;
			return;
		}
		if (value && !playerTrapped)
		{
			CryliaTrapCoordinator.ForceDestroyTrapForPlayer(player);
		}
		playerTrapped = value;
		CryliaTrapCoordinator.SetPlayerState(player, CryliaHoldSource.Cage, value);
	}
}
