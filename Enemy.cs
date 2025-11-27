using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
	public List<string> targetTags;

	[Header("Stats")]
	[Range(1f, 50f)]
	public float targetDistance = 20f;

	[Range(1f, 5f)]
	public float movementSpeed = 2f;

	private float targetSearchTime = 5f;

	private float targetSearchTimer;

	private float pathSearchTime = 0.2f;

	private float pathSearchTimer;

	private bool doneFirstPathSearch;

	protected Vector2 pathOffset;

	private float damage = 20f;

	[Range(1f, 10f)]
	public float distanceToAttackAt = 2f;

	[Range(0f, 10f)]
	public float timeBetweenAttacks;

	protected float attackTimer;

	[Range(0f, 10f)]
	public float telegraphTime;

	[Header("Misc")]
	public Animator spriteAnimator;

	public SpriteRenderer spriteRenderer;

	public List<SpriteRenderer> spritesToFlip;

	public GameObject explosionPrefab;

	public GameObject knowledgePelletPrefab;

	public bool dropRandomKnowledge = true;

	public int minimumKnowledgePellets;

	public int maximumKnowledgePellets = 2;

	private EnemyState state;

	[HideInInspector]
	public Rigidbody2D rb;

	public Health health;

	public GameObject deadPrefab;

	protected EventInstance mainSoundEffectInstance;

	private List<Enemy> commonNearbyEnemies = new List<Enemy>();

	private float stunTimer;

	private float currentStunTime;

	public GameObject stunStars;

	private float originalDrag = -1f;

	private bool dragReduced;

	public GameObject exclamationMark;

	private float fleeTimer;

	private float fleeTime = 1f;

	[HideInInspector]
	public bool stunned;

	private bool wasVisibleWhenAttackStarted;

	[HideInInspector]
	public float wallCollisionDamageMultiplier = 1f;

	[HideInInspector]
	public float knowledgeDropModifier = 1f;

	public GameObject hitWallParticlePrefab;

	private List<Vector2> path = new List<Vector2>();

	public LineRenderer pathDebugLine;

	public LayerMask inaccessibleLayerMask;

	public int wallProximityPenalty = 10;

	[Range(0.1f, 2f)]
	public float raycastRadius = 0.3f;

	public bool cancelAttackIfNoRaycast;

	public bool strafeOnAttack;

	[Range(0f, 2f)]
	public float strafeSpeed = 0.5f;

	[Range(0f, 2f)]
	public float collisionDamageModifier = 1f;

	public Light2D aura;

	private bool currentlyDying;

	private bool scheduledToDie;

	private bool hasAttackingParameter;

	private float spriteFlipTimer;

	[HideInInspector]
	public GameObject currentTarget { get; private set; }

	public EnemyState State
	{
		get
		{
			return state;
		}
		protected set
		{
			state = value;
		}
	}

	private void Start()
	{
		State = EnemyState.Idle;
		rb = GetComponent<Rigidbody2D>();
		health = GetComponent<Health>();
		health.OnDeath += Death;
		health.OnDamage += Aggro;
		health.OnDamage += RunManager.Instance.DamagedEnemy;
		pathSearchTimer = pathSearchTime;
		targetSearchTimer = targetSearchTime;
		attackTimer = -1f;
		if (spriteAnimator != null)
		{
			AnimatorControllerParameter[] parameters = spriteAnimator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].name == "Attacking")
				{
					hasAttackingParameter = true;
					break;
				}
			}
		}
		if (Difficulty.Instance != null)
		{
			float num = Difficulty.Instance.enemyPower();
			health.maxHealth = Mathf.RoundToInt(health.maxHealth * num);
			health.health = Mathf.RoundToInt(health.health * num);
		}
		GameObject obj = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		if ((object)obj != null && obj.GetComponent<SpellCasting>().enemyBuffRelic)
		{
			Debug.Log("buffed speed");
			movementSpeed *= 1.75f;
		}
		Init();
	}

	public virtual void Init()
	{
	}

	private void Update()
	{
		if (base.gameObject.GetComponent<Health>().health <= 0f && !currentlyDying && !scheduledToDie)
		{
			scheduledToDie = true;
			StartCoroutine(DeathCoroutine());
		}
		if (PlayerManager.ClosestPlayer(base.gameObject.transform.position) != null && base.gameObject.GetComponent<Health>() != null && PlayerManager.ClosestPlayer(base.gameObject.transform.position).GetComponent<SpellCasting>().enemyHealRelic && base.gameObject.GetComponent<Health>().regenRate != 2f)
		{
			base.gameObject.GetComponent<Health>().regenRate = 2f;
		}
		if (pathDebugLine != null && path != null)
		{
			pathDebugLine.positionCount = path.Count + 1;
			pathDebugLine.SetPosition(0, base.transform.position);
			for (int i = 0; i < path.Count; i++)
			{
				pathDebugLine.SetPosition(i + 1, path[i]);
			}
		}
		if (stunned)
		{
			stunTimer += Time.deltaTime;
			if (stunTimer > currentStunTime)
			{
				stunTimer = 0f;
				currentStunTime = 0f;
				stunned = false;
				stunStars?.SetActive(value: false);
				RestoreDrag();
			}
			return;
		}
		targetSearchTimer += Time.deltaTime;
		pathSearchTimer += Time.deltaTime;
		attackTimer += Time.deltaTime;
		if (targetSearchTimer >= targetSearchTime)
		{
			targetSearchTimer = 0f;
			SearchForTarget();
		}
		Tick();
		if (spriteAnimator != null && spriteAnimator.GetFloat("Speed") != 0f)
		{
			spriteAnimator.SetFloat("Speed", Mathf.Max(0.5f, rb.velocity.magnitude / 2f));
		}
		if (currentTarget == null)
		{
			State = EnemyState.Idle;
		}
		else
		{
			if (pathSearchTimer >= pathSearchTime)
			{
				if (!doneFirstPathSearch)
				{
					doneFirstPathSearch = true;
					pathSearchTimer = Random.Range(0f, pathSearchTime);
				}
				else
				{
					pathSearchTimer = 0f;
				}
				UpdatePath();
			}
			if (spritesToFlip.Count > 0)
			{
				spriteFlipTimer += Time.deltaTime;
				bool flag = false;
				flag = ((Mathf.Abs(rb.velocity.x) < 1f) ? ((currentTarget.transform.position - base.transform.position).x >= 0f) : ((double)rb.velocity.x >= 0.1));
				if (spriteFlipTimer > 0.3f)
				{
					SpriteRenderer[] array = spritesToFlip.ToArray();
					foreach (SpriteRenderer obj in array)
					{
						if (obj.flipX != flag)
						{
							spriteFlipTimer = 0f;
						}
						obj.flipX = flag;
					}
				}
			}
			Target();
		}
		switch (State)
		{
		case EnemyState.Idle:
			exclamationMark.SetActive(value: false);
			if (hasAttackingParameter && spriteAnimator != null)
			{
				spriteAnimator.SetBool("Attacking", value: false);
			}
			break;
		case EnemyState.Approaching:
			exclamationMark.SetActive(value: false);
			if (targetDistance > Vector3.Distance(currentTarget.transform.position, base.transform.position))
			{
				if (distanceToAttackAt > Vector3.Distance(currentTarget.transform.position, base.transform.position) && (!cancelAttackIfNoRaycast || canSeePlayer()))
				{
					if (IsVisibleOnScreen(0f))
					{
						wasVisibleWhenAttackStarted = true;
						ChangeState(EnemyState.Attacking);
					}
					else
					{
						pathOffset = Vector2.zero;
						Navigate();
					}
				}
				else
				{
					pathOffset = Vector2.zero;
					Navigate();
				}
			}
			else
			{
				ChangeState(EnemyState.Idle);
			}
			break;
		case EnemyState.Attacking:
			exclamationMark.SetActive(value: false);
			if (distanceToAttackAt < Vector3.Distance(currentTarget.transform.position, base.transform.position) && CancelAttack(CancelAttackReason.TargetTooFar))
			{
				wasVisibleWhenAttackStarted = false;
				ChangeState(EnemyState.Approaching);
				break;
			}
			if (cancelAttackIfNoRaycast && !canSeePlayer() && CancelAttack(CancelAttackReason.NoTargetRaycast))
			{
				wasVisibleWhenAttackStarted = false;
				ChangeState(EnemyState.Approaching);
				break;
			}
			if (!IsVisibleOnScreen() && !wasVisibleWhenAttackStarted && CancelAttack(CancelAttackReason.OffScreen))
			{
				wasVisibleWhenAttackStarted = false;
				ChangeState(EnemyState.Approaching);
				break;
			}
			if (strafeOnAttack)
			{
				Vector3 vector2 = currentTarget.transform.position - base.transform.position;
				rb.velocity = new Vector2(vector2.normalized.y, 0f - vector2.normalized.x) * (movementSpeed * strafeSpeed);
				if ((double)rb.velocity.magnitude > 0.2)
				{
					spriteAnimator.SetBool("Walking", value: true);
				}
			}
			Attack();
			break;
		case EnemyState.Fleeing:
			fleeTimer += Time.deltaTime;
			if (fleeTimer >= fleeTime)
			{
				fleeTimer = 0f;
				if (currentTarget != null)
				{
					ChangeState(EnemyState.Approaching);
				}
				else
				{
					ChangeState(EnemyState.Idle);
				}
				exclamationMark.SetActive(value: false);
				break;
			}
			exclamationMark.SetActive(value: true);
			if (currentTarget != null)
			{
				Vector2 vector = (Vector2)base.transform.position - (Vector2)currentTarget.transform.position;
				closestNearbyEnemy();
				rb.velocity = new Vector2(vector.normalized.x, vector.normalized.y) * movementSpeed;
			}
			break;
		}
		if ((double)rb.velocity.magnitude < 0.1 && spriteAnimator != null)
		{
			spriteAnimator.SetBool("Walking", value: false);
		}
	}

	protected void Navigate()
	{
		Vector2 vector = ((path.Count <= 0) ? ((Vector2)base.transform.position) : path[0]);
		if (Vector2.Distance(vector, base.transform.position) < 0.2f && path.Count > 0)
		{
			path.RemoveAt(0);
			vector = ((path.Count <= 0) ? ((Vector2)base.transform.position) : path[0]);
		}
		Vector2 vector2 = vector - (Vector2)base.transform.position;
		Enemy enemy = closestNearbyEnemy();
		Vector3 vector3 = Vector3.zero;
		if (enemy != null)
		{
			vector3 = base.transform.position - enemy.transform.position;
		}
		rb.velocity = new Vector2(vector2.normalized.x + vector3.normalized.x / 3f, vector2.normalized.y + vector3.normalized.y / 3f) * movementSpeed;
		if (spriteAnimator != null)
		{
			spriteAnimator.SetBool("Walking", value: true);
		}
	}

	private bool canSeePlayer()
	{
		if (Vector2.Distance(currentTarget.transform.position, base.transform.position) < 1f)
		{
			return true;
		}
		Vector2 direction = (currentTarget.transform.position - base.transform.position).normalized;
		RaycastHit2D raycastHit2D = Physics2D.CircleCast(base.transform.position, raycastRadius, direction, distanceToAttackAt + 1f, inaccessibleLayerMask);
		if (raycastHit2D.collider != null && raycastHit2D.transform.tag == "Player")
		{
			return true;
		}
		return false;
	}

	private bool IsVisibleOnScreen(float padding = 0.1f)
	{
		if (Camera.main == null)
		{
			Debug.LogWarning("No camera found, allowing attack to prevent breaking gameplay");
			return true;
		}
		Vector3 vector = Camera.main.WorldToViewportPoint(base.transform.position);
		if (vector.x >= 0f - padding && vector.x <= 1f + padding && vector.y >= 0f - padding && vector.y <= 1f + padding)
		{
			return vector.z > 0f;
		}
		return false;
	}

	public virtual void Target()
	{
	}

	public virtual void Tick()
	{
	}

	public abstract void Attack();

	public virtual bool CancelAttack(CancelAttackReason cancelAttackReason)
	{
		return true;
	}

	public void ChangeState(EnemyState newState)
	{
		if (State == newState)
		{
			return;
		}
		State = newState;
		switch (State)
		{
		case EnemyState.Idle:
			attackTimer = 0f;
			wasVisibleWhenAttackStarted = false;
			break;
		case EnemyState.Approaching:
			attackTimer = 0f;
			wasVisibleWhenAttackStarted = false;
			break;
		case EnemyState.Fleeing:
			fleeTimer = 0f;
			wasVisibleWhenAttackStarted = false;
			if (CancelAttack(CancelAttackReason.Fleeing))
			{
				ChangeState(EnemyState.Approaching);
			}
			break;
		case EnemyState.Attacking:
			break;
		}
	}

	public void OnPathFound(Vector2[] newPath, bool success)
	{
		path = new List<Vector2>();
		if (currentTarget != null)
		{
			if (!success)
			{
				newPath = new Vector2[1] { currentTarget.transform.position + (Vector3)pathOffset };
			}
			for (int i = 0; i < newPath.Length; i++)
			{
				path.Add(newPath[i]);
			}
		}
	}

	protected void UpdatePath()
	{
		if (path == null || path.Count == 0)
		{
			path = new List<Vector2>();
			path.Add(currentTarget.transform.position);
		}
		PathRequestManager.RequestPath(base.transform.position, (Vector2)currentTarget.transform.position + pathOffset, OnPathFound, wallProximityPenalty);
	}

	private void SearchForTarget()
	{
		GameObject gameObject = null;
		float num = float.PositiveInfinity;
		foreach (string targetTag in targetTags)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(targetTag);
			foreach (GameObject gameObject2 in array)
			{
				float num2 = Vector3.Distance(base.transform.position, gameObject2.transform.position);
				if (num2 < targetDistance && num2 < num)
				{
					num = num2;
					gameObject = gameObject2;
				}
			}
		}
		if (gameObject != null)
		{
			currentTarget = gameObject;
			if (State == EnemyState.Idle)
			{
				ChangeState(EnemyState.Approaching);
			}
		}
		commonNearbyEnemies = new List<Enemy>();
		Enemy[] array2 = Object.FindObjectsOfType<Enemy>();
		foreach (Enemy enemy in array2)
		{
			if (enemy == this)
			{
				break;
			}
			if (enemy.GetType() == GetType() && Vector3.Distance(enemy.transform.position, base.transform.position) < 5f)
			{
				commonNearbyEnemies.Add(enemy);
			}
		}
	}

	private Enemy closestNearbyEnemy()
	{
		Enemy result = null;
		float num = 2f;
		Enemy[] array = commonNearbyEnemies.ToArray();
		foreach (Enemy enemy in array)
		{
			if (enemy != null)
			{
				float num2 = Vector3.Distance(base.transform.position, enemy.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = enemy;
				}
			}
		}
		return result;
	}

	private IEnumerator DeathCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		if (!currentlyDying)
		{
			Debug.Log("Death due to scheduled death");
			Death(null);
		}
	}

	private void Death(GameObject attacker)
	{
		if (currentlyDying)
		{
			return;
		}
		currentlyDying = true;
		if (GravityMain.IsGravityPushedEnemy(base.gameObject))
		{
			GravityMain.OnGravityPushedEnemyDied(base.gameObject);
		}
		Screenshake.Instance.AddTrauma(0.5f);
		Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity);
		if (dropRandomKnowledge && !Object.FindObjectOfType<AnarchyManager>())
		{
			int num = Mathf.RoundToInt((float)Random.Range(minimumKnowledgePellets, maximumKnowledgePellets + 1) * knowledgeDropModifier);
			num *= FloorManager.Instance.currentLoop;
			for (int i = 0; i <= num; i++)
			{
				Object.Instantiate(knowledgePelletPrefab, base.transform.position + new Vector3(Random.insideUnitCircle.x * 0.5f, Random.insideUnitCircle.y * 0.5f), Quaternion.identity);
			}
		}
		RunManager.Instance?.KilledEnemy(attacker, base.gameObject);
		if (mainSoundEffectInstance.isValid())
		{
			mainSoundEffectInstance.stop(STOP_MODE.IMMEDIATE);
		}
		if (deadPrefab != null)
		{
			Object.Instantiate(deadPrefab, base.transform.position - new Vector3(0f, 0.09f, 0f), Quaternion.identity);
			SceneManager.MoveGameObjectToScene(deadPrefab, base.gameObject.scene);
		}
		Dead();
		Object.Destroy(base.gameObject);
	}

	public virtual void Dead()
	{
	}

	public void DoAttackDamage()
	{
		if (!stunned)
		{
			int num = 1;
			if (Difficulty.Instance != null)
			{
				num = Difficulty.Instance.damageMultiplier;
			}
			currentTarget.GetComponent<Health>().Damage(damage * (float)num, base.gameObject);
		}
	}

	private void Aggro(float damage, bool causedDeath, GameObject beingAttacked, GameObject attacker = null)
	{
		if (causedDeath)
		{
			return;
		}
		TookDamage();
		if (attacker == null)
		{
			currentTarget = PlayerManager.ClosestPlayer(base.transform.position);
			if (State == EnemyState.Idle)
			{
				ChangeState(EnemyState.Approaching);
			}
			if (State == EnemyState.Fleeing)
			{
				ChangeState(EnemyState.Idle);
			}
		}
		else if (attacker.transform.tag == "Player")
		{
			currentTarget = attacker;
			if (State == EnemyState.Idle)
			{
				ChangeState(EnemyState.Approaching);
			}
			if (State == EnemyState.Fleeing)
			{
				ChangeState(EnemyState.Idle);
			}
		}
	}

	public virtual void TookDamage()
	{
	}

	public virtual void Stunned()
	{
	}

	public void Stun(float stunTime, bool doStunStars = true)
	{
		currentStunTime += stunTime;
		stunned = true;
		if (doStunStars)
		{
			stunStars?.SetActive(value: true);
		}
		if (stunTime > 1f && CancelAttack(CancelAttackReason.Stunned))
		{
			ChangeState(EnemyState.Approaching);
		}
		ReduceDragDuringStun();
	}

	public void Unstun()
	{
		stunTimer = 0f;
		currentStunTime = 0f;
		stunned = false;
		stunStars?.SetActive(value: false);
		RestoreDrag();
	}

	private void ReduceDragDuringStun()
	{
		if (rb != null && !dragReduced)
		{
			originalDrag = rb.drag;
			rb.drag = 4f;
			dragReduced = true;
		}
	}

	private void RestoreDrag()
	{
		if (rb != null && dragReduced && originalDrag >= 0f)
		{
			rb.drag = originalDrag;
			dragReduced = false;
		}
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.layer != 18 && col.gameObject.layer != 20)
		{
			return;
		}
		if (col.relativeVelocity.magnitude > 6f)
		{
			GetComponent<Health>().Damage(Mathf.RoundToInt((col.relativeVelocity.magnitude - 4f) / 2f * wallCollisionDamageMultiplier * collisionDamageModifier));
			Object.Instantiate(hitWallParticlePrefab, base.transform.position, Quaternion.identity);
		}
		OnHitWall(col.relativeVelocity.magnitude);
		if (GravityMain.IsGravityPushedEnemy(base.gameObject))
		{
			if (health != null && health.health > 0f)
			{
				GravityMain.OnGravityPushedEnemyHitWall(base.gameObject);
			}
			else
			{
				GravityMain.OnGravityPushedEnemyDied(base.gameObject);
			}
		}
	}

	public virtual void OnHitWall(float speed)
	{
	}
}
