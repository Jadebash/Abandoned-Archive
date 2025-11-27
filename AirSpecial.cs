using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Air Special")]
public class AirSpecial : SpecialSpell
{
	public GameObject windBurstEffect;

	public GameObject airPulseEffect;

	private GameObject airPulseInstance;

	public GameObject airTrail;

	private GameObject airTrailInstance;

	private GameObject rollAirTrailInstance;

	public GameObject airRadiusEffect;

	private GameObject prefabInstance;

	private SpriteRenderer targetSpriteRenderer;

	private GameObject player;

	public float maxHoldDown = 4f;

	public float travelTime = 0.2f;

	public float invincibilityTime = 0.5f;

	public float landingDamageRadius = 2f;

	public LayerMask raycastLayerMask;

	public LayerMask damageLayerMask;

	[HideInInspector]
	public bool rechargeOnKill;

	[HideInInspector]
	public bool damageOnBurst;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			rechargeOnKill = true;
			break;
		case 2:
			damage *= 1.5f;
			cooldownTime *= 1.1f;
			break;
		case 3:
			invincibilityTime += 0.5f;
			break;
		case 4:
			cooldownTime *= 0.85f;
			break;
		case 5:
			damageOnBurst = true;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		this.player = player;
		if (prefabInstance == null)
		{
			isCoroutineRunning = true;
		}
		chargeTimer += Time.deltaTime;
		Screenshake.Instance.AddTrauma(Time.deltaTime * 2f);
		if (prefabInstance == null)
		{
			Screenshake.Instance.AddTrauma(0.6f);
			prefabInstance = Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
			Object.Instantiate(windBurstEffect, player.transform.position, Quaternion.identity);
			airPulseInstance = Object.Instantiate(airPulseEffect, player.transform.position, Quaternion.identity);
			player.GetComponent<Movement>().enabled = false;
			player.GetComponent<Health>().invincible = true;
			targetSpriteRenderer = prefabInstance.GetComponent<SpriteRenderer>();
			player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
			player.GetComponent<Movement>().anim.SetBool("fly", value: true);
			ControllerAim componentInChildren = player.GetComponentInChildren<ControllerAim>();
			if (componentInChildren != null)
			{
				componentInChildren.hideCrosshair = true;
				if (componentInChildren.controllerCrosshair != null)
				{
					componentInChildren.controllerCrosshair.gameObject.SetActive(value: false);
				}
			}
			Collider2D[] array = Physics2D.OverlapCircleAll(player.transform.position, landingDamageRadius, damageLayerMask.value);
			foreach (Collider2D collider2D in array)
			{
				if ((bool)collider2D.GetComponent<Health>())
				{
					if (damageOnBurst)
					{
						collider2D.GetComponent<Health>().Damage(Mathf.RoundToInt(damage * damageMultiplier * 0.5f));
					}
					Vector3 normalized = (collider2D.transform.position - player.transform.position).normalized;
					if ((bool)collider2D.GetComponent<Enemy>())
					{
						collider2D.GetComponent<Enemy>().Stun(0.2f);
						collider2D.GetComponent<Rigidbody2D>().AddForce(normalized * 750f);
						collider2D.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(collider2D.GetComponent<Rigidbody2D>().velocity, 15f);
					}
				}
			}
		}
		targetPoint = ClampTargetPointToScreenBounds(targetPoint);
		if (PlayerManager.players.Length > 1 && CameraController.Instance != null)
		{
			GameObject gameObject = null;
			GameObject[] players = PlayerManager.players;
			foreach (GameObject gameObject2 in players)
			{
				if (gameObject2 == player)
				{
					gameObject = gameObject2;
					break;
				}
			}
			if (gameObject != null)
			{
				float maxCoopDistance = CameraController.Instance.GetMaxCoopDistance();
				if (Vector3.Distance(targetPoint, gameObject.transform.position) > maxCoopDistance)
				{
					Vector3 normalized2 = (targetPoint - gameObject.transform.position).normalized;
					targetPoint = gameObject.transform.position + normalized2 * maxCoopDistance;
				}
			}
		}
		prefabInstance.transform.position = targetPoint;
		Vector3 vector = prefabInstance.transform.position - player.transform.position;
		if (Physics2D.CircleCast(player.transform.position, 0.3f, vector.normalized, Vector2.Distance(player.transform.position, prefabInstance.transform.position), raycastLayerMask.value).collider == null)
		{
			targetSpriteRenderer.color = Color.white;
		}
		else
		{
			targetSpriteRenderer.color = Color.red;
		}
		if (chargeTimer >= maxHoldDown)
		{
			Charged(damageMultiplier);
		}
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		Screenshake.Instance.AddTrauma(0.7f);
		Vector3 normalized = (prefabInstance.transform.position - player.transform.position).normalized;
		RaycastHit2D raycastHit2D = Physics2D.CircleCast(player.transform.position, 0.3f, normalized, Vector2.Distance(player.transform.position, prefabInstance.transform.position), raycastLayerMask.value);
		airTrailInstance = Object.Instantiate(airTrail, player.transform);
		airTrailInstance.GetComponent<DestroyAfterTime>().time = travelTime * 2f;
		player.layer = 17;
		if (raycastHit2D.collider == null)
		{
			player.transform.LeanMove(prefabInstance.transform.position, travelTime);
			CoroutineEmitter.Instance.StartCoroutine(DamageLandingArea(prefabInstance.transform.position, damageMultiplier));
		}
		else
		{
			player.transform.LeanMove(raycastHit2D.point + new Vector2(normalized.x, normalized.y) * -0.5f, travelTime);
			CoroutineEmitter.Instance.StartCoroutine(DamageLandingArea(raycastHit2D.point + new Vector2(normalized.x, normalized.y) * -0.5f, damageMultiplier));
		}
		Object.Destroy(airPulseInstance);
		Object.Destroy(prefabInstance);
		prefabInstance = null;
		base.Charged();
		player.GetComponent<Health>().invincible = false;
		player.GetComponent<Health>().invincibilityTimer = player.GetComponent<Health>().invincibilityTime - (invincibilityTime + travelTime);
		player.GetComponent<Health>().invincibilityFrames = true;
	}

	private IEnumerator DamageLandingArea(Vector3 landingSpot, float damageMultiplier)
	{
		yield return new WaitForSeconds(travelTime);
		Movement component = player.GetComponent<Movement>();
		component.anim.SetBool("fly", value: false);
		ControllerAim componentInChildren = player.GetComponentInChildren<ControllerAim>();
		if (componentInChildren != null)
		{
			componentInChildren.hideCrosshair = false;
		}
		component.enabled = true;
		player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		component.anim.SetTrigger("idle");
		airTrailInstance.transform.parent = null;
		Object.Instantiate(airRadiusEffect, landingSpot, Quaternion.identity);
		player.layer = 8;
		Screenshake.Instance.AddTrauma(0.9f);
		Collider2D[] array = Physics2D.OverlapCircleAll(landingSpot, landingDamageRadius, damageLayerMask.value);
		foreach (Collider2D collider2D in array)
		{
			if ((bool)collider2D.GetComponent<Health>())
			{
				if (collider2D.GetComponent<Health>().Damage(damage * damageMultiplier) && rechargeOnKill)
				{
					player.GetComponent<SpellCasting>().SetSpellCooldownTime(this, cooldownTime);
				}
				Vector3 normalized = (collider2D.transform.position - landingSpot).normalized;
				if ((bool)collider2D.GetComponent<Enemy>())
				{
					collider2D.GetComponent<Enemy>().Stun(0.2f);
					collider2D.GetComponent<Rigidbody2D>().AddForce(normalized * 1000f);
				}
			}
		}
		isCoroutineRunning = false;
	}

	public override void Roll(GameObject player)
	{
		Movement component = player.GetComponent<Movement>();
		component.flying = true;
		Object.Instantiate(windBurstEffect, player.transform.position, Quaternion.identity);
		rollAirTrailInstance = Object.Instantiate(airTrail, player.transform);
		rollAirTrailInstance.GetComponent<DestroyAfterTime>().time = component.rollCooldown * 2f;
		CoroutineEmitter.Instance.StartCoroutine(RollWait(component.rollCooldown, component));
	}

	private IEnumerator RollWait(float rollLength, Movement movement)
	{
		yield return new WaitForSeconds(rollLength);
		rollAirTrailInstance.transform.parent = null;
		movement.flying = false;
	}

	private Vector3 ClampTargetPointToScreenBounds(Vector3 worldPoint)
	{
		if (Camera.main == null)
		{
			return worldPoint;
		}
		Vector3 position = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		Vector3 vector = Camera.main.ScreenToWorldPoint(position);
		vector.z = worldPoint.z;
		float num = Camera.main.orthographicSize * 2f * Camera.main.aspect * 0.45f;
		Vector3 vector2 = worldPoint - vector;
		if (vector2.magnitude > num)
		{
			vector2 = vector2.normalized * num;
			worldPoint = vector + vector2;
		}
		Vector3 position2 = Camera.main.WorldToScreenPoint(worldPoint);
		position2.x = Mathf.Clamp(position2.x, 0f, Screen.width);
		position2.y = Mathf.Clamp(position2.y, 0f, Screen.height);
		worldPoint = Camera.main.ScreenToWorldPoint(position2);
		worldPoint.z = vector.z;
		return worldPoint;
	}
}
