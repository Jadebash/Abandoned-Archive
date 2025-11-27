using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Lemures : MonoBehaviour
{
	private Animator anim;

	private Rigidbody2D rb;

	[HideInInspector]
	public Vector3 startingPosition;

	[HideInInspector]
	public Health health;

	public float walkSpeed = 2f;

	private bool flipped;

	public List<SpriteRenderer> sprites;

	public List<GameObject> minions;

	private List<GameObject> activeMinions = new List<GameObject>();

	public Transform spiritsRed;

	public Transform spiritsBlue;

	public Transform spiritsGreen;

	public Transform flameBalls;

	public Transform beam;

	public Transform tripleBeam;

	public LayerMask teleportLayerMask;

	public LayerMask validTeleportLayerMask;

	public Transform damageTriggerDash;

	public int numOfAttacks;

	private bool rotateBeam;

	public Transform astralForm;

	[HideInInspector]
	public float blueSpiritTimer;

	public bool blackSword;

	public GameObject flameWall;

	public EventReference sfx;

	private bool minionDead;

	private int count;

	private AttackHelper attackHelper;

	private void Start()
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		health = GetComponent<Health>();
		startingPosition = base.transform.position;
		attackHelper = GetComponent<AttackHelper>();
		if (attackHelper == null)
		{
			attackHelper = base.gameObject.AddComponent<AttackHelper>();
		}
	}

	private void FixedUpdate()
	{
		if (attackHelper != null)
		{
			rb.position = attackHelper.ClampPositionToBounds(rb.position);
			GameObject[] players = PlayerManager.players;
			foreach (GameObject gameObject in players)
			{
				gameObject.GetComponent<Rigidbody2D>().position = attackHelper.ClampPositionToBounds(gameObject.GetComponent<Rigidbody2D>().position);
			}
		}
	}

	private void Update()
	{
		if (base.gameObject.GetComponent<Boss>() == null && !Object.FindObjectOfType<Boss>())
		{
			Object.Destroy(base.gameObject);
		}
		if (base.gameObject.GetComponent<Boss>() != null && health.health <= 0f)
		{
			foreach (GameObject activeMinion in activeMinions)
			{
				if (!(activeMinion == null))
				{
					activeMinion.GetComponent<Health>().health = 0f;
				}
			}
		}
		MinionDeath();
		if (minionDead || health.health <= 0f)
		{
			return;
		}
		Transform transform = PlayerManager.ClosestPlayer(base.transform.position)?.transform;
		if (transform != null && rb.velocity.magnitude < 0.3f)
		{
			if ((base.transform.position - transform.position).x >= 0f && !flipped)
			{
				flipped = true;
			}
			else if ((base.transform.position - transform.position).x < 0f && flipped)
			{
				flipped = false;
			}
		}
		else if (rb.velocity.x < 0f && !flipped)
		{
			flipped = true;
		}
		else if (rb.velocity.x >= 0f && flipped)
		{
			flipped = false;
		}
		foreach (SpriteRenderer sprite in sprites)
		{
			sprite.flipX = flipped;
		}
		damageTriggerDash.localScale = new Vector3((!flipped) ? 1 : (-1), 1f, 1f);
		if (rotateBeam && transform != null)
		{
			Vector2 vector = (Vector2)transform.transform.position + transform.GetComponent<Rigidbody2D>().velocity * 0.15f - (Vector2)base.transform.position;
			float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			beam.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		blueSpiritTimer += Time.deltaTime;
	}

	public void MinionDeath()
	{
		if (base.gameObject.GetComponent<Boss>() == null && base.gameObject.GetComponent<Health>().health <= 0f && !minionDead)
		{
			minionDead = true;
			GetComponent<Health>().invincible = false;
			rb.velocity = Vector2.zero;
			rb.isKinematic = true;
			if (GetComponent<Enemy>() != null)
			{
				GetComponent<Enemy>().enabled = false;
			}
			Animator component = base.gameObject.GetComponent<Animator>();
			if (component != null)
			{
				component.SetBool("Walking", value: false);
				component.SetTrigger("Idle");
				component.SetFloat("Speed", 0f);
				component.SetTrigger("Dead");
			}
			Object.FindObjectOfType<Boss>().GetComponent<Lemures>().count++;
		}
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	public void ResetPosition()
	{
		rb.velocity = Vector2.zero;
		base.transform.position = startingPosition;
	}

	public void SpawnNextMinion()
	{
		GameObject gameObject = minions[Random.Range(0, minions.Count)];
		minions.Remove(gameObject);
		gameObject.GetComponent<Health>().invincible = false;
		gameObject.GetComponent<Animator>().SetBool("Intro", value: true);
		gameObject.GetComponent<Animator>().enabled = true;
		gameObject.gameObject.tag = "Enemy";
		activeMinions.Add(gameObject);
	}

	public void StartBeam()
	{
		Transform transform = PlayerManager.ClosestPlayer(base.transform.position).transform;
		Vector2 vector = (Vector2)transform.transform.position + transform.GetComponent<Rigidbody2D>().velocity * 0.33f - (Vector2)base.transform.position;
		float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		beam.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void StartTripleBeam()
	{
		Transform transform = PlayerManager.ClosestPlayer(base.transform.position).transform;
		Vector2 vector = (Vector2)transform.transform.position + transform.GetComponent<Rigidbody2D>().velocity * 0.33f - (Vector2)base.transform.position;
		float angle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		tripleBeam.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void StopBeam()
	{
		rotateBeam = false;
	}

	public void DamagePlayer()
	{
		PlayerManager.ClosestPlayer(base.transform.position).GetComponent<Health>().Damage(20f, base.gameObject);
		Screenshake.Instance.AddTrauma(0.9f);
	}

	public void DisableActiveMinions()
	{
		foreach (GameObject activeMinion in activeMinions)
		{
			if (!(activeMinion == null))
			{
				activeMinion?.SetActive(value: false);
			}
		}
		CameraController.Instance.biasTarget = null;
	}

	public void EnableActiveMinions()
	{
		foreach (GameObject activeMinion in activeMinions)
		{
			if (!(activeMinion == null))
			{
				activeMinion?.SetActive(value: true);
			}
		}
		CameraController.Instance.biasTarget = base.transform.parent;
		Screenshake.Instance.AddTrauma(0.8f);
	}

	public void EnableBlackSword()
	{
		blackSword = true;
		Object.Instantiate(flameWall, PlayerManager.ClosestPlayer(base.gameObject.transform.position).transform.position, Quaternion.identity);
	}

	public void SpawnSpiritsRed()
	{
		StartCoroutine(SpiritSpawningRed());
	}

	public void SpawnSpiritsGreen()
	{
		StartCoroutine(SpiritSpawningGreen());
	}

	public void SpawnSpiritsBlue()
	{
		blueSpiritTimer = 0f;
		spiritsBlue.gameObject.SetActive(value: true);
		StartCoroutine(SpiritSpawningBlue());
	}

	public void SpawnFireBalls()
	{
		blackSword = true;
		StartCoroutine(spawningFlameBalls());
	}

	private IEnumerator SpiritSpawningRed()
	{
		RuntimeManager.PlayOneShot(sfx);
		Transform player = PlayerManager.ClosestPlayer(base.transform.position).transform;
		foreach (Transform item in spiritsRed)
		{
			item.gameObject.SetActive(value: false);
		}
		foreach (Transform item2 in spiritsRed)
		{
			item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
			item2.position = attackHelper.ClampPositionToBounds(item2.position);
			int num = 0;
			while (Vector2.Distance(item2.position, player.position) < 3f)
			{
				item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
				item2.position = attackHelper.ClampPositionToBounds(item2.position);
				num++;
				if (num > 100)
				{
					Debug.LogWarning("Couldnt find valid position for spirit.");
					break;
				}
			}
			item2.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(2f / (float)spiritsRed.childCount);
		}
	}

	private IEnumerator spawningFlameBalls()
	{
		_ = PlayerManager.ClosestPlayer(base.transform.position).transform;
		foreach (Transform flameBall in flameBalls)
		{
			flameBall.position = base.gameObject.transform.position + (Vector3)Random.insideUnitCircle * 2f;
			flameBall.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator SpiritSpawningGreen()
	{
		RuntimeManager.PlayOneShot(sfx);
		Transform player = PlayerManager.ClosestPlayer(base.transform.position).transform;
		foreach (Transform item in spiritsGreen)
		{
			item.gameObject.SetActive(value: false);
		}
		foreach (Transform item2 in spiritsGreen)
		{
			item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
			item2.position = attackHelper.ClampPositionToBounds(item2.position);
			int num = 0;
			while (Vector2.Distance(item2.position, player.position) < 3f)
			{
				item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
				item2.position = attackHelper.ClampPositionToBounds(item2.position);
				num++;
				if (num > 100)
				{
					Debug.LogWarning("Couldnt find valid position for spirit.");
					break;
				}
			}
			item2.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(2f / (float)spiritsGreen.childCount);
		}
	}

	private IEnumerator SpiritSpawningBlue()
	{
		RuntimeManager.PlayOneShot(sfx);
		Transform transform = Object.Instantiate(spiritsBlue.gameObject, base.transform.position, Quaternion.identity).transform;
		Transform player = PlayerManager.ClosestPlayer(base.transform.position).transform;
		foreach (Transform item in transform)
		{
			item.gameObject.SetActive(value: false);
		}
		foreach (Transform item2 in transform)
		{
			item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
			item2.position = attackHelper.ClampPositionToBounds(item2.position);
			int num = 0;
			while (Vector2.Distance(item2.position, player.position) < 3f)
			{
				item2.position = player.position + (Vector3)Random.insideUnitCircle * 5f;
				item2.position = attackHelper.ClampPositionToBounds(item2.position);
				num++;
				if (num > 100)
				{
					Debug.LogWarning("Couldnt find valid position for spirit.");
					break;
				}
			}
			item2.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(2f / (float)spiritsBlue.childCount);
		}
	}
}
