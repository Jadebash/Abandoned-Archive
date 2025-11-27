using System.Collections;
using FMODUnity;
using UnityEngine;

public class Rufus : MonoBehaviour
{
	public int numOfAttacks;

	public GameObject rods;

	private GameObject rodDamageArea;

	public GameObject tetheringLine;

	private Rigidbody2D rb;

	private float jumpForce = 8f;

	public GameObject jumpAOE;

	public GameObject arm;

	[HideInInspector]
	public Vector3 startingPos;

	public GameObject boundaries;

	public GameObject floorBoard;

	public GameObject shadow;

	private float shadowOffset;

	private AttackHelper attackHelper;

	private void Start()
	{
		startingPos = base.transform.position;
		rb = GetComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Kinematic;
		attackHelper = GetComponent<AttackHelper>();
		if (attackHelper == null)
		{
			attackHelper = base.gameObject.AddComponent<AttackHelper>();
		}
		shadowOffset = shadow.transform.localPosition.y;
	}

	private void Update()
	{
	}

	private void FlameSweep()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusFireSpin", base.transform.position);
	}

	private void FlameSummon()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusFire", base.transform.position);
	}

	private void ThrowRods()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (base.gameObject.transform.position.x > gameObject.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		Vector3 vector = gameObject.transform.position - base.transform.position;
		GameObject gameObject2 = Object.Instantiate(rods, base.transform.position, Quaternion.identity);
		gameObject2.transform.up = -vector;
		rodDamageArea = gameObject2.transform.Find("DamageArea").gameObject;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ArmWoosh", base.transform.position);
	}

	private IEnumerator StrikeRods()
	{
		RufusLightningRod[] array = Object.FindObjectsOfType<RufusLightningRod>();
		foreach (RufusLightningRod rufusLightningRod in array)
		{
			GameObject obj = Object.Instantiate(tetheringLine);
			obj.GetComponent<TetheringLine>().from = base.transform;
			obj.GetComponent<TetheringLine>().to = rufusLightningRod.transform;
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusLightning", base.transform.position);
			Screenshake.Instance.AddTrauma(0.5f);
		}
		rodDamageArea.SetActive(value: true);
		yield return new WaitForSeconds(0.1f);
		rodDamageArea.SetActive(value: false);
		if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	private void LightningMelee()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		GameObject obj = Object.Instantiate(tetheringLine);
		obj.GetComponent<TetheringLine>().from = base.transform;
		obj.GetComponent<TetheringLine>().to = gameObject.transform;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusLightning", base.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
		if (!gameObject.GetComponent<Movement>().rolling)
		{
			gameObject.GetComponent<Health>().Damage(20f);
		}
	}

	private IEnumerator JumpAttack()
	{
		Random.Range(0, 2);
		StartCoroutine(Jump());
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(Jump());
		GetComponent<Animator>().SetBool("Walking", value: true);
	}

	private IEnumerator Jump()
	{
		GameObject player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		if (base.gameObject.transform.position.x > player.transform.position.x && !base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.mass = 10000f;
		Vector3 lockedShadowWorldPos = shadow.transform.position;
		_ = shadow.transform.localPosition;
		Transform shadowParent = shadow.transform.parent;
		shadow.transform.SetParent(null);
		rb.AddForce(base.transform.up * 2f * rb.mass, ForceMode2D.Impulse);
		float risingTimer = 0f;
		while (risingTimer < 0.75f)
		{
			risingTimer += Time.deltaTime;
			Vector3 position = shadow.transform.position;
			position.y = lockedShadowWorldPos.y;
			shadow.transform.position = position;
			yield return null;
		}
		Vector3 vector = player.transform.position + new Vector3(0f, 0.5f, 0f) - base.gameObject.transform.position;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusWoosh", base.transform.position);
		rb.AddForce(-rb.velocity * rb.mass, ForceMode2D.Impulse);
		rb.AddForce(vector * 5f * rb.mass, ForceMode2D.Impulse);
		shadow.transform.SetParent(shadowParent, worldPositionStays: true);
		float dashStartLocalY = shadow.transform.localPosition.y;
		float dashTimer = 0f;
		while (dashTimer < 0.25f)
		{
			dashTimer += Time.deltaTime;
			float t = dashTimer / 0.25f;
			Vector3 localPosition = shadow.transform.localPosition;
			localPosition.y = Mathf.Lerp(dashStartLocalY, shadowOffset, t);
			shadow.transform.localPosition = localPosition;
			yield return null;
		}
		shadow.transform.localPosition = new Vector3(0f, shadowOffset, 0f);
		jumpAOE.SetActive(value: true);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusHittingFloor", base.transform.position);
		rb.AddForce(-rb.velocity * rb.mass, ForceMode2D.Impulse);
		rb.bodyType = RigidbodyType2D.Kinematic;
		Screenshake.Instance.AddTrauma(0.5f);
		yield return new WaitForSeconds(0.1f);
		jumpAOE.SetActive(value: false);
		yield return new WaitForSeconds(0.5f);
		if (base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	private void ThrowArm()
	{
		GameObject gameObject = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		if (gameObject == null)
		{
			GetComponent<Animator>().SetTrigger("CancelThrow");
			return;
		}
		GameObject gameObject2 = Object.Instantiate(arm, base.gameObject.transform.position, Quaternion.identity);
		Vector3 vector = gameObject.transform.position - gameObject2.transform.position;
		gameObject2.GetComponent<RufusArm>().direction = vector.normalized;
		GetComponent<Animator>().SetBool("FollowingArm", value: true);
	}

	private IEnumerator LightningDischarge()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusLightning", base.transform.position);
		Screenshake.Instance.AddTrauma(0.5f);
		jumpAOE.SetActive(value: true);
		yield return new WaitForSeconds(0.1f);
		jumpAOE.SetActive(value: false);
		GetComponent<Animator>().SetBool("Walking", value: true);
	}

	private void StrikePlayer()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusLightning", base.transform.position);
		GameObject gameObject = base.transform.Find("LightningCaster").gameObject;
		GameObject gameObject2 = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		GameObject obj = Object.Instantiate(tetheringLine);
		obj.GetComponent<TetheringLine>().from = gameObject.transform;
		obj.GetComponent<TetheringLine>().to = gameObject2.transform;
		gameObject2.GetComponent<Health>().Damage(20f);
		GetComponent<Animator>().SetBool("Walking", value: true);
	}

	private void StompGround()
	{
		for (int i = 0; i < 10; i++)
		{
			float x = Random.Range(-6.5f, 6.5f);
			float y = Random.Range(-2f, 13f);
			GameObject gameObject = Object.Instantiate(floorBoard, base.gameObject.transform.position, Quaternion.identity);
			gameObject.transform.position += new Vector3(x, y, 0f);
			RufusFloorboard[] array = Object.FindObjectsOfType<RufusFloorboard>();
			foreach (RufusFloorboard rufusFloorboard in array)
			{
				if ((double)Vector2.Distance(gameObject.transform.position, rufusFloorboard.gameObject.transform.position) <= 0.75)
				{
					x = Random.Range(-6.5f, 6.5f);
					y = Random.Range(-2f, 13f);
					gameObject.transform.position = base.gameObject.transform.position;
					gameObject.transform.position += new Vector3(x, y, 0f);
				}
			}
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RufusHittingFloor", base.transform.position);
		Screenshake.Instance.AddTrauma(0.5f);
	}

	private void FlipSprite()
	{
		if (!base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX)
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
		}
		else
		{
			base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	public Vector3 ClampPositionToBounds(Vector3 position)
	{
		if (attackHelper != null)
		{
			return attackHelper.ClampPositionToBounds(position);
		}
		return position;
	}

	private void OnDestroy()
	{
		if (shadow != null)
		{
			Object.Destroy(shadow);
		}
	}
}
