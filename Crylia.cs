using System.Collections;
using FMODUnity;
using UnityEngine;

public class Crylia : MonoBehaviour
{
	private GameObject player;

	private Animator anim;

	[HideInInspector]
	public Vector3 dir;

	public GameObject[] hospitalEquipment;

	public GameObject claw;

	public GameObject cageShadow;

	public GameObject trapShadow;

	public ImageBorderHighlight clawBorderHighlight;

	[HideInInspector]
	public Vector3 startingPos;

	private bool trappedPlayer;

	[HideInInspector]
	public bool kickedPlayer;

	[HideInInspector]
	public bool cageMoving = true;

	[HideInInspector]
	public bool slowedPlayer;

	private float slowedPlayerTime;

	public int numOfAttacks;

	private bool stunned;

	private float stunnedTime;

	private Vector3 stunnedPos;

	private bool pouncing;

	public GameObject kennel;

	public GameObject nail;

	public GameObject nailContainer;

	public GameObject darkness;

	public GameObject[] syringes;

	public GameObject psychedelic;

	private AttackHelper attackHelper;

	private CryliaCharge chargeBehaviour;

	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		startingPos = base.gameObject.transform.position;
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		anim = GetComponent<Animator>();
		attackHelper = GetComponent<AttackHelper>();
		chargeBehaviour = anim.GetBehaviour<CryliaCharge>();
		spriteRenderer = base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
	}

	public Vector3 ClampPositionToBounds(Vector3 position)
	{
		if (attackHelper != null)
		{
			return attackHelper.ClampPositionToBounds(position);
		}
		return position;
	}

	public bool IsPositionInBounds(Vector3 position)
	{
		if (attackHelper == null)
		{
			return true;
		}
		Vector3 b = attackHelper.ClampPositionToBounds(position);
		return Vector3.Distance(position, b) < 0.001f;
	}

	private void Update()
	{
		bool flag = anim.GetCurrentAnimatorStateInfo(0).IsName("Charge") || anim.GetCurrentAnimatorStateInfo(0).IsTag("Charge");
		if (!anim.GetBool("Walking") && !anim.GetBool("WalkToCage") && chargeBehaviour != null && !chargeBehaviour.charging && !pouncing && !flag)
		{
			base.gameObject.transform.position = startingPos;
		}
		if (stunned)
		{
			stunnedTime -= Time.deltaTime;
			if (stunnedTime > 0f)
			{
				base.gameObject.transform.position = stunnedPos;
			}
			else
			{
				stunned = false;
			}
		}
		if (slowedPlayer)
		{
			slowedPlayerTime += Time.deltaTime;
			if (slowedPlayerTime >= 5f)
			{
				slowedPlayer = false;
				slowedPlayerTime = 0f;
			}
		}
	}

	public void KickCage()
	{
		GameObject gameObject = GameObject.Find("CryliaCage(Clone)");
		gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		Vector3 right = base.transform.right;
		float num = 25f;
		if (attackHelper != null)
		{
			Vector3 position = gameObject.transform.position + right * num * 0.5f;
			if (!IsPositionInBounds(position))
			{
				num *= 0.5f;
			}
		}
		gameObject.GetComponent<Rigidbody2D>().AddForce(right * num, ForceMode2D.Impulse);
		kickedPlayer = true;
		anim.SetBool("Walking", value: true);
	}

	private void FlipSprite()
	{
		if (player.transform.position.x < base.gameObject.transform.position.x && !spriteRenderer.flipX)
		{
			spriteRenderer.flipX = true;
		}
		else if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}

	private void ResetFlip()
	{
		if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}

	public void ThrowGreenSyringe()
	{
		GameObject gameObject = Object.Instantiate(syringes[3], base.transform.position, Quaternion.identity);
		Vector3 vector = CalculateThrowTarget(gameObject.transform.position);
		dir = vector - gameObject.transform.position;
		gameObject.transform.up = dir.normalized;
		gameObject.SetActive(value: true);
	}

	public void GreenSyringeEffect()
	{
		if (GameObject.Find("Psychedelic(Clone)") == null)
		{
			Object.Instantiate(psychedelic);
		}
	}

	public void setPlayerTrapped()
	{
		anim.SetBool("WalkToCage", value: true);
	}

	public void stopPlayerTrapped()
	{
		anim.SetBool("WalkToCage", value: false);
	}

	public void ThrowCage()
	{
		Object.Instantiate(cageShadow, base.gameObject.transform.position, Quaternion.identity);
	}

	public void ThrowTrap()
	{
		Object.Instantiate(trapShadow, base.gameObject.transform.position, Quaternion.identity);
	}

	private void FlipThrowEquipment()
	{
		if (player.transform.position.x < base.gameObject.transform.position.x && !spriteRenderer.flipX)
		{
			spriteRenderer.flipX = true;
		}
		else if (player.transform.position.x > base.gameObject.transform.position.x && spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}

	public Vector3 CalculateThrowTarget(Vector3 projectilePosition)
	{
		Vector3 result = player.transform.position;
		if (Random.Range(0, 2) == 1)
		{
			Rigidbody2D component = player.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				float num = Random.Range(0.2f, 0.4f);
				result = player.transform.position + (Vector3)component.velocity * num;
			}
		}
		return result;
	}

	public void throwEquipment()
	{
		int num = Random.Range(0, 3);
		Object.Instantiate(hospitalEquipment[num], base.gameObject.transform.position, Quaternion.identity).transform.up = dir;
	}

	public void Stun(float time)
	{
		stunned = true;
		stunnedTime = time;
		stunnedPos = base.gameObject.transform.position;
	}

	public void ClawAttack()
	{
		clawBorderHighlight.SingleBurst();
		Object.Instantiate(claw, player.transform.position, Quaternion.identity);
	}

	public void BlindPlayer()
	{
		if (GameObject.Find("Darkness(Clone)") == null)
		{
			Object.Instantiate(darkness);
		}
	}

	public void RandomOneShotAttack()
	{
		if (Random.Range(0, 2) == 0)
		{
			anim.SetTrigger("OneShotTraps");
			return;
		}
		int syringe = Random.Range(0, syringes.Length);
		ThrowSyringe(syringe);
	}

	private void ThrowSyringe(int syringe)
	{
		switch (syringe)
		{
		case 0:
			anim.SetTrigger("OneShotBlueSyringe");
			break;
		case 1:
			anim.SetTrigger("OneShotRedSyringe");
			break;
		case 2:
			anim.SetTrigger("OneShotBlackSyringe");
			break;
		case 3:
			anim.SetTrigger("OneShotGreenSyringe");
			break;
		}
	}

	private void ThrowBlueSyringe()
	{
		GameObject gameObject = Object.Instantiate(syringes[1], base.transform.position, Quaternion.identity);
		Vector3 vector = CalculateThrowTarget(gameObject.transform.position);
		dir = vector - gameObject.transform.position;
		gameObject.transform.up = dir.normalized;
		gameObject.SetActive(value: true);
	}

	private void ThrowRedSyringe()
	{
		GameObject gameObject = Object.Instantiate(syringes[0], base.transform.position, Quaternion.identity);
		Vector3 vector = CalculateThrowTarget(gameObject.transform.position);
		dir = vector - gameObject.transform.position;
		gameObject.transform.up = dir.normalized;
		gameObject.SetActive(value: true);
	}

	private void ThrowBlackSyringe()
	{
		GameObject gameObject = Object.Instantiate(syringes[2], base.transform.position, Quaternion.identity);
		Vector3 vector = CalculateThrowTarget(gameObject.transform.position);
		dir = vector - gameObject.transform.position;
		gameObject.transform.up = dir.normalized;
		gameObject.SetActive(value: true);
	}

	private IEnumerator Pounce()
	{
		if (player.transform.position.x < base.gameObject.transform.position.x && !spriteRenderer.flipX)
		{
			spriteRenderer.flipX = true;
		}
		pouncing = true;
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.AddForce((player.transform.position + new Vector3(-1.25f, 0f, 0f) - base.gameObject.transform.position).normalized * 20f, ForceMode2D.Impulse);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Woosh", base.transform.position);
		yield return new WaitForSeconds(0.2f);
		rb.velocity = new Vector2(0f, 0f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/SmallThud", base.transform.position);
		Screenshake.Instance.AddTrauma(0.5f);
		base.transform.position = ClampPositionToBounds(base.transform.position);
		if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}

	private IEnumerator FinalPounce()
	{
		if (player.transform.position.x < base.gameObject.transform.position.x && !spriteRenderer.flipX)
		{
			spriteRenderer.flipX = true;
		}
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		rb.AddForce((player.transform.position - base.gameObject.transform.position).normalized * 20f, ForceMode2D.Impulse);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Woosh", base.transform.position);
		yield return new WaitForSeconds(0.2f);
		rb.velocity = new Vector2(0f, 0f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/SmallThud", base.transform.position);
		Screenshake.Instance.AddTrauma(0.5f);
		base.transform.position = ClampPositionToBounds(base.transform.position);
		anim.SetBool("Walking", value: true);
		rb.bodyType = RigidbodyType2D.Kinematic;
		pouncing = false;
		if (spriteRenderer.flipX)
		{
			spriteRenderer.flipX = false;
		}
	}

	private void ThrowKennel()
	{
		float x = Random.Range(-4f, 4f);
		float y = Random.Range(-4f, 4f);
		Object.Instantiate(kennel, startingPos + new Vector3(x, y, 0f), Quaternion.identity);
	}

	private void ThrowNails()
	{
		if (Random.Range(0, 2) == 0)
		{
			float num = -2.5f;
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = new Vector3(num, 0f, 0f);
				GameObject gameObject = Object.Instantiate(nail, base.gameObject.transform.position + vector, Quaternion.identity);
				gameObject.transform.up = player.transform.position - gameObject.transform.position;
				num += 1f;
			}
		}
		else
		{
			GameObject gameObject2 = Object.Instantiate(nailContainer, base.gameObject.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
			gameObject2.transform.up = player.transform.position - gameObject2.transform.position;
		}
	}

	private void RemoveDarkness()
	{
		darkness.SetActive(value: false);
	}
}
