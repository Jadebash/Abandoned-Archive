using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
	public delegate void RollCallback();

	public delegate void TeleportCallback();

	[Serializable]
	public class SpeedEffect
	{
		public float amount;

		public float duration;

		public float timeRemaining;

		public string sourceId;

		public SpeedEffect(float amount, float duration, string sourceId = "")
		{
			this.amount = amount;
			this.duration = duration;
			timeRemaining = duration;
			this.sourceId = sourceId;
		}
	}

	[Header("Movement")]
	public float speed = 2f;

	[HideInInspector]
	public float speedModifier = 1f;

	[HideInInspector]
	public bool rollCancel;

	[HideInInspector]
	public bool rollCancelled;

	private Rigidbody2D rb;

	[Header("Rolls")]
	private SpellCasting spellCasting;

	public float rollCooldown = 1f;

	public float rollDuration = 0.5f;

	private float rollTimer;

	private float rollDurationTimer;

	public float rollInvinciblePercentage = 50f;

	public float rollSpeedModifier = 1.5f;

	private float startingRollSpeedModifier;

	[HideInInspector]
	public bool rolling;

	private Health health;

	public Vector2 targetVelocity;

	public Vector2 desiredVelocity;

	private Vector2 idleDirection;

	[Header("Animation")]
	public Animator anim;

	public Animator rollPressAnim;

	public Animator upgradePressAnim;

	public Image upgradeButton;

	public GameObject upgradeLabel;

	private Image rollSliderImage;

	public bool lookAtCursor;

	[Header("Teleport")]
	public Image teleportButton;

	public Animator teleportButtonAnimator;

	public GameObject teleportLabel;

	public float teleportThreshold = 13f;

	public Vector3 teleportBasePosition = new Vector3(0f, 0f, 0f);

	private float teleportTimer;

	public float teleportCooldown = 2f;

	public GameObject teleportEffectPrefab;

	[HideInInspector]
	public bool canTeleportToNextFloor;

	[Header("Text")]
	public LocalisedString rollName;

	public LocalisedString rollDescription;

	public LocalisedString teleportName;

	public LocalisedString teleportDescription;

	private Vector3 previousTeleportPosition = new Vector3(0f, 0f, 0f);

	private Vector3 teleportPosition = new Vector3(0f, 0f, 0f);

	private bool canTeleportBack;

	public bool canTeleport = true;

	[HideInInspector]
	public bool isTeleporting;

	[HideInInspector]
	public bool flying;

	private bool stunned;

	private float stunTimer;

	private float stunTime;

	private ControllerAim aim;

	private float distanceToPlayer;

	private bool pushedPlayer;

	private float pushbackCooldown = 0.5f;

	private Enemy[] enemies;

	private Boss boss;

	private GameObject pushingEnemy;

	private float checkForEnemies;

	public bool inverted;

	private float invertedTimer = 10f;

	public Material invertedGlow;

	public Material invertedEndingGlow;

	public Material defaultPlayer;

	private bool setGlow;

	private AnarchyManager anarchyManager;

	private Tutorial tutorial;

	public bool speedChanged;

	public float sunSpecialModifier;

	public float intensityModifier;

	[Header("Boundary Detection")]
	public LayerMask wallLayerMask = -1;

	public float boundaryCheckRadius = 0.1f;

	private Vector3 lastValidPosition;

	private List<SpeedEffect> activeSpeedEffects = new List<SpeedEffect>();

	private bool wasSpellCastingEnabled;

	public event RollCallback OnRoll;

	public event RollCallback OnTeleport;

	public void SetTargetVelocity(Vector2 newTargetVelocity)
	{
		targetVelocity = newTargetVelocity;
	}

	public void AddSpeedEffect(float amount, float duration, string sourceId = "")
	{
		activeSpeedEffects.Add(new SpeedEffect(amount, duration, sourceId));
	}

	public void RemoveSpeedEffect(string sourceId, bool removeAll = false)
	{
		if (removeAll)
		{
			activeSpeedEffects.RemoveAll((SpeedEffect effect) => effect.sourceId == sourceId);
			return;
		}
		List<SpeedEffect> list = (from effect in activeSpeedEffects
			where effect.sourceId == sourceId
			orderby effect.timeRemaining
			select effect).ToList();
		if (list.Count > 0)
		{
			activeSpeedEffects.Remove(list.First());
		}
	}

	public bool HasSpeedEffect(string sourceId)
	{
		return activeSpeedEffects.Any((SpeedEffect effect) => effect.sourceId == sourceId);
	}

	private bool IsPositionInsideWall(Vector3 position)
	{
		Collider2D collider2D = Physics2D.OverlapCircle(position, boundaryCheckRadius, wallLayerMask);
		if (collider2D != null)
		{
			return !collider2D.isTrigger;
		}
		return false;
	}

	private bool IsPositionInvalid(Vector3 currentPosition, bool skipRaycast = false)
	{
		if (IsPositionInsideWall(currentPosition))
		{
			return true;
		}
		if (!skipRaycast)
		{
			Vector3 vector = currentPosition - lastValidPosition;
			float magnitude = vector.magnitude;
			if (magnitude > 0.01f)
			{
				vector.Normalize();
				if (Physics2D.Raycast(lastValidPosition, vector, magnitude, wallLayerMask).collider != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void CheckAndCorrectBounds()
	{
		if (isTeleporting)
		{
			return;
		}
		if (IsPositionInvalid(base.transform.position))
		{
			if (!IsPositionInvalid(lastValidPosition, skipRaycast: true))
			{
				base.transform.position = lastValidPosition;
				rb.velocity = Vector2.zero;
			}
			else
			{
				lastValidPosition = base.transform.position;
			}
		}
		else
		{
			lastValidPosition = base.transform.position;
		}
	}

	public void SafeTeleport(Vector3 newPosition)
	{
		base.transform.position = newPosition;
		lastValidPosition = newPosition;
		rb.velocity = Vector2.zero;
	}

	private void Start()
	{
		enemies = UnityEngine.Object.FindObjectsOfType<Enemy>();
		rb = GetComponent<Rigidbody2D>();
		rollTimer = rollCooldown;
		health = GetComponent<Health>();
		spellCasting = GetComponent<SpellCasting>();
		if (Manager.Instance != null)
		{
			Manager.Instance.OnPause += OnPause;
		}
		startingRollSpeedModifier = rollSpeedModifier - 1f;
		aim = GetComponentInChildren<ControllerAim>();
		UpdateManagerReferences();
		lastValidPosition = base.transform.position;
		if (FloorManager.Instance != null && FloorManager.Instance.isLoadingFloor)
		{
			isTeleporting = true;
		}
	}

	private void UpdateManagerReferences()
	{
		anarchyManager = UnityEngine.Object.FindObjectOfType<AnarchyManager>();
		tutorial = UnityEngine.Object.FindObjectOfType<Tutorial>();
		FindIconAnimators();
	}

	private void FindIconAnimators()
	{
		PlayerManager instance = PlayerManager.Instance;
		if (instance != null)
		{
			Transform transform = instance.transform.Find("LTPress");
			if (transform != null)
			{
				rollPressAnim = transform.GetComponent<Animator>();
			}
			Transform transform2 = instance.transform.Find("SpellSlotSkillUpgrade");
			if (transform2 != null)
			{
				upgradePressAnim = transform2.GetComponent<Animator>();
			}
		}
	}

	private void FixedUpdate()
	{
		RuntimeManager.StudioSystem.getParameterByName("Intensity", out var value);
		if (isTeleporting)
		{
			rb.velocity = Vector2.zero;
			desiredVelocity = Vector2.zero;
			targetVelocity = Vector2.zero;
			anim.SetTrigger("idle");
			return;
		}
		if (value == 0f && !speedChanged)
		{
			intensityModifier = 0.5f;
			speedChanged = true;
			if (PlayerManager.players.Length > 1)
			{
				GameObject[] players = PlayerManager.players;
				foreach (GameObject gameObject in players)
				{
					if (gameObject != base.gameObject)
					{
						gameObject.GetComponent<Movement>().intensityModifier = 0.5f;
						gameObject.GetComponent<Movement>().speedChanged = true;
					}
				}
			}
		}
		else if (value > 0f && speedChanged)
		{
			intensityModifier = 0f;
			speedChanged = false;
			if (PlayerManager.players.Length > 1)
			{
				GameObject[] players = PlayerManager.players;
				foreach (GameObject gameObject2 in players)
				{
					if (gameObject2 != base.gameObject)
					{
						gameObject2.GetComponent<Movement>().intensityModifier = 0f;
						gameObject2.GetComponent<Movement>().speedChanged = false;
					}
				}
			}
		}
		if (inverted)
		{
			if (!setGlow)
			{
				base.gameObject.transform.Find("Model").GetComponent<SpriteRenderer>().material = invertedGlow;
				setGlow = true;
			}
			invertedTimer -= Time.deltaTime;
			if (invertedTimer <= 0.5f)
			{
				base.gameObject.transform.Find("Model").GetComponent<SpriteRenderer>().material = invertedEndingGlow;
			}
			else
			{
				base.gameObject.transform.Find("Model").GetComponent<SpriteRenderer>().material = invertedGlow;
			}
			if (invertedTimer <= 0f)
			{
				Debug.Log("Not inverted");
				base.gameObject.transform.Find("Model").GetComponent<SpriteRenderer>().material = defaultPlayer;
				setGlow = false;
				inverted = false;
				invertedTimer = 10f;
			}
		}
		checkForEnemies += Time.deltaTime;
		if (boss == null && anarchyManager == null && tutorial == null)
		{
			if (checkForEnemies >= 2.5f)
			{
				checkForEnemies = 0f;
				enemies = UnityEngine.Object.FindObjectsOfType<Enemy>();
				boss = UnityEngine.Object.FindObjectOfType<Boss>();
				UpdateManagerReferences();
			}
			if (pushbackCooldown > 0f)
			{
				pushbackCooldown -= Time.deltaTime;
			}
			else if (pushedPlayer)
			{
				pushedPlayer = false;
			}
			Vector2 vector = base.transform.position;
			float num = 1f;
			Enemy[] array = enemies;
			foreach (Enemy enemy in array)
			{
				if (!(enemy == null) && !(enemy.health == null) && !(enemy.health.health <= 0f))
				{
					float sqrMagnitude = ((Vector2)enemy.transform.position - vector).sqrMagnitude;
					if (sqrMagnitude <= num && health.pushBack && !pushedPlayer && pushbackCooldown <= 0f)
					{
						pushingEnemy = enemy.gameObject;
						pushedPlayer = true;
						pushPlayer();
						pushbackCooldown = 0.5f;
					}
					else if (sqrMagnitude > num && pushedPlayer)
					{
						pushedPlayer = false;
					}
				}
			}
		}
		if (boss != null)
		{
			if (Vector2.Distance(base.gameObject.transform.position, boss.gameObject.transform.position) <= 1f && health.pushBack && !pushedPlayer && pushbackCooldown <= 0f)
			{
				pushedPlayer = true;
				pushingEnemy = boss.gameObject;
				pushPlayer(isBoss: true);
				pushbackCooldown = 0.5f;
			}
			if (Vector2.Distance(base.gameObject.transform.position, boss.gameObject.transform.position) > 1f && pushedPlayer)
			{
				pushedPlayer = false;
			}
			if (boss.gameObject.GetComponent<Health>().health <= 0f)
			{
				boss = null;
			}
		}
		for (int num2 = activeSpeedEffects.Count - 1; num2 >= 0; num2--)
		{
			activeSpeedEffects[num2].timeRemaining -= Time.deltaTime;
			if (activeSpeedEffects[num2].timeRemaining <= 0f)
			{
				activeSpeedEffects.RemoveAt(num2);
			}
		}
		float num3 = 0f;
		foreach (SpeedEffect activeSpeedEffect in activeSpeedEffects)
		{
			num3 += activeSpeedEffect.amount;
		}
		float num4 = Mathf.Clamp(num3 + speedModifier + sunSpecialModifier + intensityModifier, 0.1f, 3f);
		anim.SetFloat("speedMultiplier", num4);
		rollTimer += Time.deltaTime;
		rollDurationTimer += Time.deltaTime;
		teleportTimer -= Time.deltaTime;
		_ = (100f - rollInvinciblePercentage) / 2f;
		if (rollTimer < rollCooldown * (rollInvinciblePercentage / 100f))
		{
			health.invincible = true;
			if (flying)
			{
				base.gameObject.layer = 17;
			}
			else
			{
				base.gameObject.layer = 10;
			}
		}
		else
		{
			health.invincible = false;
			base.gameObject.layer = 8;
		}
		if (!rolling)
		{
			if (canTeleport && ((WaveManager.Instance != null && !WaveManager.Instance.playerInRoom) || WaveManager.Instance == null) && teleportTimer <= 0f)
			{
				if (Vector3.Distance(base.transform.position, new Vector3(0f, 0f, 0f)) > teleportThreshold)
				{
					canTeleportBack = false;
				}
				if (canTeleportToNextFloor)
				{
					teleportButton.color = Color.yellow;
					teleportLabel.SetActive(value: true);
					teleportButton.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				else if (Vector3.Distance(base.transform.position, new Vector3(0f, 0f, 0f)) > teleportThreshold && !canTeleportBack)
				{
					teleportButton.color = Color.white;
					teleportLabel.SetActive(value: true);
					teleportButton.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				else if (Vector3.Distance(base.transform.position, new Vector3(0f, 0f, 0f)) < teleportThreshold && canTeleportBack)
				{
					teleportButton.color = Color.white;
					teleportLabel.SetActive(value: true);
					teleportButton.transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				else
				{
					teleportButton.color = Color.grey;
					teleportLabel.SetActive(value: false);
					teleportButton.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			else
			{
				teleportButton.color = Color.grey;
				teleportLabel.SetActive(value: false);
				teleportButton.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			anim.SetBool("rolling", value: false);
			if (rollTimer >= rollCooldown)
			{
				rollCancelled = false;
			}
		}
		else
		{
			teleportButton.color = Color.grey;
			teleportLabel.SetActive(value: false);
			teleportButton.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (!rolling)
		{
			targetVelocity = desiredVelocity;
		}
		else if (targetVelocity == Vector2.zero)
		{
			targetVelocity = idleDirection;
		}
		if (targetVelocity == Vector2.zero)
		{
			anim.SetTrigger("idle");
		}
		else
		{
			float num5 = Mathf.Abs(targetVelocity[0]);
			float num6 = Mathf.Abs(targetVelocity[1]);
			if (!inverted)
			{
				if (num5 >= num6)
				{
					if (targetVelocity[0] > 0f)
					{
						anim.SetTrigger("right");
						idleDirection = Vector2.right;
					}
					else
					{
						anim.SetTrigger("left");
						idleDirection = -Vector2.right;
					}
				}
				else if (targetVelocity[1] > 0f)
				{
					anim.SetTrigger("up");
					idleDirection = Vector2.up;
				}
				else
				{
					anim.SetTrigger("down");
					idleDirection = -Vector2.up;
				}
			}
			else if (num5 >= num6)
			{
				if (targetVelocity[0] < 0f)
				{
					anim.SetTrigger("right");
					idleDirection = Vector2.right;
				}
				else
				{
					anim.SetTrigger("left");
					idleDirection = -Vector2.right;
				}
			}
			else if (targetVelocity[1] < 0f)
			{
				anim.SetTrigger("up");
				idleDirection = Vector2.up;
			}
			else
			{
				anim.SetTrigger("down");
				idleDirection = -Vector2.up;
			}
		}
		Vector2 vector2 = (inverted ? (-targetVelocity * speed * num4) : (targetVelocity * speed * num4));
		if (spellCasting.rechargeSpeedBuff)
		{
			vector2 *= spellCasting.movementSpeedBuff;
			EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.Speed, num4 * spellCasting.movementSpeedBuff);
		}
		else
		{
			EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.Speed, num4);
		}
		EffectsVisualiser.Instance?.SetEffectVisual(EffectsVisualiser.EffectType.Roll, rollSpeedModifier - startingRollSpeedModifier);
		if (rollTimer >= rollDuration)
		{
			rolling = false;
			rollDurationTimer = 0f;
		}
		if (rolling)
		{
			anim.SetBool("rolling", value: true);
			vector2 = Mathf.Sin((rollDuration - rollDurationTimer) / rollDuration * (MathF.PI / 2f) + MathF.PI / 8f) * vector2 * rollSpeedModifier;
		}
		if (stunned)
		{
			stunTimer += Time.deltaTime;
			if (!(stunTimer >= stunTime))
			{
				CheckAndCorrectBounds();
				return;
			}
			stunned = false;
			stunTimer = 0f;
			stunTime = 0f;
		}
		rb.velocity = vector2;
		CheckAndCorrectBounds();
	}

	public void pushPlayer(bool isBoss = false)
	{
		if (UnityEngine.Random.Range(1, 3) != 1)
		{
			return;
		}
		Vector3 vector = base.gameObject.transform.position - pushingEnemy.transform.position;
		if (!isBoss)
		{
			Stun(0.1f);
			rb.AddForce(vector.normalized * 75f, ForceMode2D.Impulse);
			pushingEnemy.GetComponent<Enemy>().Stun(0.5f, doStunStars: false);
			Rigidbody2D component = pushingEnemy.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				component.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
				component.AddForce(-vector.normalized * 32f, ForceMode2D.Impulse);
				component.velocity = Vector2.ClampMagnitude(component.velocity, 15f);
			}
		}
		else
		{
			rb.AddForce(vector.normalized * 100f, ForceMode2D.Impulse);
		}
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Deflect", base.transform.position);
	}

	private void UpdateDissolveValue(float newDissolve)
	{
		anim.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", newDissolve);
		if (newDissolve != 1f)
		{
			return;
		}
		isTeleporting = true;
		this.OnTeleport?.Invoke();
		if (canTeleportToNextFloor)
		{
			Vector3 position = Vector3.zero;
			if (CameraController.Instance != null)
			{
				position = CameraController.Instance.transform.position;
			}
			lastValidPosition = new Vector3(0f, 0f, 0f);
			base.transform.position = new Vector3(0f, 0f, 0f);
			if (CameraController.Instance != null)
			{
				CameraController.Instance.LockAtPosition(position);
			}
			FloorManager.Instance.NextFloor();
			canTeleportToNextFloor = false;
			spellCasting.canCast = true;
		}
		else
		{
			lastValidPosition = teleportPosition;
			base.transform.position = teleportPosition;
			if (teleportPosition == new Vector3(0f, 0f, 0f))
			{
				canTeleportBack = true;
			}
			else
			{
				canTeleportBack = false;
			}
			anim.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", 0f);
			health.invincibilityFrames = true;
			health.invincibilityTimer = 0f;
			if (wasSpellCastingEnabled)
			{
				spellCasting.canCast = true;
			}
			RoomOptimiser[] array = UnityEngine.Object.FindObjectsOfType<RoomOptimiser>(includeInactive: true);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Optimise(null);
			}
			ShadowOptimiser[] array2 = UnityEngine.Object.FindObjectsOfType<ShadowOptimiser>();
			LightOptimiser[] array3 = UnityEngine.Object.FindObjectsOfType<LightOptimiser>();
			ShadowCasterOptimiser[] array4 = UnityEngine.Object.FindObjectsOfType<ShadowCasterOptimiser>();
			ShadowOptimiser[] array5 = array2;
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i].UpdateLight();
			}
			LightOptimiser[] array6 = array3;
			for (int i = 0; i < array6.Length; i++)
			{
				array6[i].UpdateLight();
			}
			ShadowCasterOptimiser[] array7 = array4;
			for (int i = 0; i < array7.Length; i++)
			{
				array7[i].UpdateCaster();
			}
		}
		isTeleporting = false;
	}

	public void Stun(float time)
	{
		stunned = true;
		stunTime += time;
	}

	public void OnPause(bool paused)
	{
		if (paused && !rolling)
		{
			targetVelocity = Vector2.zero;
		}
	}

	public void Move(InputAction.CallbackContext context)
	{
		Vector2 vector = context.ReadValue<Vector2>();
		desiredVelocity = vector;
	}

	public void Roll(InputAction.CallbackContext context)
	{
		if (!base.enabled || !context.started || Time.timeScale == 0f || (!(rollTimer >= rollCooldown) && (!rollCancel || !rolling || rollCancelled || !(rollTimer >= rollDuration * 0.5f))))
		{
			return;
		}
		if (rollCancel && rolling && !rollCancelled)
		{
			rollCancelled = true;
			targetVelocity = desiredVelocity;
			anim.ResetTrigger("right");
			anim.ResetTrigger("left");
			anim.ResetTrigger("up");
			anim.ResetTrigger("down");
			float num = Mathf.Abs(targetVelocity[0]);
			float num2 = Mathf.Abs(targetVelocity[1]);
			if (num >= num2)
			{
				if (targetVelocity[0] > 0f)
				{
					anim.SetTrigger("right");
				}
				else
				{
					anim.SetTrigger("left");
				}
			}
			else if (targetVelocity[1] > 0f)
			{
				anim.SetTrigger("up");
			}
			else
			{
				anim.SetTrigger("down");
			}
			anim.SetTrigger("forceRoll");
		}
		rollPressAnim.SetTrigger("Press");
		rollTimer = 0f;
		rollDurationTimer = 0f;
		rolling = true;
		Screenshake.Instance.AddTrauma(0.3f);
		anim.SetTrigger("roll");
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Roll", base.transform.position);
		if (spellCasting.currentStance.spellSlots[0].spell != null)
		{
			((SpecialSpell)spellCasting.currentStance.spellSlots[0].spell).Roll(base.gameObject);
		}
		this.OnRoll?.Invoke();
	}

	public void Teleport(InputAction.CallbackContext context)
	{
		if (!base.enabled || !context.started || Time.timeScale == 0f || !(SceneManager.GetActiveScene().name != "Floor 4") || (WaveManager.Instance != null && WaveManager.Instance.playerInRoom) || !canTeleport || !(teleportTimer <= 0f))
		{
			return;
		}
		if (spellCasting == null)
		{
			spellCasting = GetComponent<SpellCasting>();
		}
		if (spellCasting == null)
		{
			return;
		}
		MusicManager.Calm();
		wasSpellCastingEnabled = spellCasting.canCast;
		if (canTeleportToNextFloor || (Vector3.Distance(base.transform.position, teleportBasePosition) > teleportThreshold && !canTeleportBack) || (Vector3.Distance(base.transform.position, teleportBasePosition) < teleportThreshold && canTeleportBack))
		{
			spellCasting.canCast = false;
		}
		if (canTeleportToNextFloor)
		{
			isTeleporting = true;
			if (teleportButtonAnimator != null)
			{
				teleportButtonAnimator.SetTrigger("Press");
			}
			LeanTween.value(base.gameObject, UpdateDissolveValue, 0f, 1f, 2.4f);
			teleportPosition = new Vector3(0f, 0f, 0f);
			previousTeleportPosition = base.transform.position;
			teleportTimer = teleportCooldown;
			UnityEngine.Object.Instantiate(teleportEffectPrefab, base.transform);
			Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}
		else if (Vector3.Distance(base.transform.position, teleportBasePosition) > teleportThreshold && !canTeleportBack)
		{
			if (PlayerManager.players.Length > 1)
			{
				GameObject[] players = PlayerManager.players;
				foreach (GameObject gameObject in players)
				{
					if (gameObject != base.gameObject)
					{
						gameObject.GetComponent<Movement>().ForceTeleport(teleportBasePosition);
					}
				}
			}
			isTeleporting = true;
			if (teleportButtonAnimator != null)
			{
				teleportButtonAnimator.SetTrigger("Press");
			}
			LeanTween.value(base.gameObject, UpdateDissolveValue, 0f, 1f, 0.4f);
			teleportPosition = teleportBasePosition;
			previousTeleportPosition = base.transform.position;
			teleportTimer = teleportCooldown;
			UnityEngine.Object.Instantiate(teleportEffectPrefab, base.transform);
		}
		else
		{
			if (!(Vector3.Distance(base.transform.position, teleportBasePosition) < teleportThreshold) || !canTeleportBack)
			{
				return;
			}
			if (PlayerManager.players.Length > 1)
			{
				GameObject[] players = PlayerManager.players;
				foreach (GameObject gameObject2 in players)
				{
					if (gameObject2 != base.gameObject)
					{
						gameObject2.GetComponent<Movement>().ForceTeleport(previousTeleportPosition);
					}
				}
			}
			isTeleporting = true;
			if (teleportButtonAnimator != null)
			{
				teleportButtonAnimator.SetTrigger("Press");
			}
			LeanTween.value(base.gameObject, UpdateDissolveValue, 0f, 1f, 0.4f);
			teleportPosition = previousTeleportPosition;
			teleportTimer = teleportCooldown;
			UnityEngine.Object.Instantiate(teleportEffectPrefab, base.transform);
		}
	}

	public void ForceTeleport(Vector3 position)
	{
		isTeleporting = true;
		LeanTween.value(base.gameObject, UpdateDissolveValue, 0f, 1f, 0.4f);
		teleportPosition = position;
		previousTeleportPosition = base.transform.position;
		teleportTimer = teleportCooldown;
		UnityEngine.Object.Instantiate(teleportEffectPrefab, base.transform);
	}

	public void Look(InputAction.CallbackContext context)
	{
		if (context.canceled || !base.enabled || rolling || !(targetVelocity == Vector2.zero) || Time.timeScale == 0f)
		{
			return;
		}
		Vector3 vector = default(Vector3);
		if (aim != null && aim.usingController)
		{
			vector = context.ReadValue<Vector2>();
			vector.x /= 2f;
			vector.y /= 2f;
		}
		else
		{
			vector = Camera.main.ScreenToViewportPoint(context.ReadValue<Vector2>());
			vector.x = Mathf.Clamp(vector.x, 0f, 1f) - 0.5f;
			vector.y = Mathf.Clamp(vector.y, 0f, 1f) - 0.5f;
		}
		if (Mathf.Abs(vector.x) < 0.1f)
		{
			vector.x = 0f;
		}
		if (Mathf.Abs(vector.y) < 0.1f)
		{
			vector.y = 0f;
		}
		if (!(vector.magnitude > 0f) || !lookAtCursor)
		{
			return;
		}
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			if (vector.x > 0f)
			{
				anim.SetTrigger("lookRight");
				idleDirection = Vector2.right;
			}
			else
			{
				anim.SetTrigger("lookLeft");
				idleDirection = -Vector2.right;
			}
		}
		else if (vector.y > 0f)
		{
			anim.SetTrigger("lookUp");
			idleDirection = Vector2.up;
		}
		else
		{
			anim.SetTrigger("lookDown");
			idleDirection = -Vector2.up;
		}
	}

	private void OnDestroy()
	{
		if (Manager.Instance != null)
		{
			Manager.Instance.OnPause -= OnPause;
		}
	}

	public void OnDisable()
	{
		string[] array = new string[4] { "right", "left", "up", "down" };
		foreach (string text in array)
		{
			anim.ResetTrigger(text);
		}
		anim.SetTrigger("idle");
		targetVelocity = Vector2.zero;
	}
}
