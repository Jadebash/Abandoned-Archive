using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Holite : MonoBehaviour
{
	private Animator anim;

	private Health health;

	private Rigidbody2D rb;

	private Vector3 startingPosition;

	private bool flipped;

	public List<SpriteRenderer> sprites;

	[Header("Prefabs")]
	public GameObject lightningSparksPrefab;

	public GameObject lightningPrefab;

	public GameObject javelinPrefab;

	public GameObject poisonBallPrefab;

	private List<GameObject> javelinInstances = new List<GameObject>();

	public LayerMask teleportLayerMask;

	public LayerMask validTeleportLayerMask;

	private GameObject[] players;

	private void Start()
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		health = GetComponent<Health>();
		players = PlayerManager.players;
		startingPosition = base.transform.position;
	}

	public Transform ClosestPlayer()
	{
		return PlayerManager.ClosestPlayer(base.transform.position)?.transform;
	}

	private void Update()
	{
		Transform transform = ClosestPlayer();
		if (transform == null || health.health <= 0f)
		{
			return;
		}
		if ((base.transform.position - transform.position).x >= 0f && !flipped)
		{
			flipped = true;
			{
				foreach (SpriteRenderer sprite in sprites)
				{
					sprite.flipX = flipped;
				}
				return;
			}
		}
		if (!((base.transform.position - transform.position).x < 0f) || !flipped)
		{
			return;
		}
		flipped = false;
		foreach (SpriteRenderer sprite2 in sprites)
		{
			sprite2.flipX = flipped;
		}
	}

	public void ResetPosition()
	{
		rb.velocity = Vector2.zero;
		base.transform.position = startingPosition;
	}

	public void TeleportCharge()
	{
		float num = 7f;
		Transform transform = ClosestPlayer();
		Vector2 vector = (Vector2)transform.transform.position + Random.insideUnitCircle * num;
		int num2 = 0;
		while (Physics2D.OverlapCircleAll(vector, 0.25f, teleportLayerMask).Length != 0 || Physics2D.OverlapCircleAll(vector, 0.1f, validTeleportLayerMask).Length == 0 || Vector2.Distance(vector, transform.position) < 3.5f)
		{
			vector = (Vector2)transform.transform.position + Random.insideUnitCircle * num;
			num2++;
			if (num2 > 100)
			{
				Debug.LogWarning("Holite could not find teleport position.");
				break;
			}
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Poof", base.transform.position);
		base.transform.position = vector;
		Screenshake.Instance.AddTrauma(0.6f);
		rb.velocity = Vector2.zero;
	}

	public void TeleportRun()
	{
		Transform transform = ClosestPlayer();
		rb.velocity = ((Vector2)transform.position - (Vector2)base.transform.position).normalized * 7.5f;
	}

	private IEnumerator LightningStrike()
	{
		Transform transform = ClosestPlayer();
		GameObject sparksInstance = Object.Instantiate(lightningSparksPrefab, transform.position + (Vector3)Random.insideUnitCircle * 1.5f, Quaternion.identity);
		yield return new WaitForSeconds(0.8f);
		Screenshake.Instance.AddTrauma(0.9f);
		Object.Instantiate(lightningPrefab, sparksInstance.transform.position, Quaternion.identity);
	}

	private void RollBall()
	{
		Transform obj = ClosestPlayer();
		Vector3 position = base.transform.position;
		Vector2 vector = obj.position - position;
		Vector2 force = vector.normalized * 5f * anim.speed;
		Object.Instantiate(poisonBallPrefab, position + (Vector3)(vector.normalized * 0.8f), Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
		Screenshake.Instance.AddTrauma(0.9f);
	}

	private IEnumerator FireJavelins()
	{
		int multiplier = 1;
		if (Random.Range(0, 2) == 0)
		{
			multiplier = -1;
		}
		ThrowJavelin(-3f * (float)multiplier);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(-1.5f * (float)multiplier);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(0f);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(1.5f * (float)multiplier);
		yield return new WaitForSeconds(0.1f);
		ThrowJavelin(3f * (float)multiplier);
	}

	private void FireRandomJavelin()
	{
		switch (Random.Range(0, 5))
		{
		case 0:
			ThrowJavelin(-3.5f);
			ThrowJavelin(-2f);
			ThrowJavelin(0.5f);
			break;
		case 1:
			ThrowJavelin(-2.5f);
			ThrowJavelin(-1f);
			ThrowJavelin(0.5f);
			break;
		case 2:
			ThrowJavelin(-1.5f);
			ThrowJavelin(0f);
			ThrowJavelin(1.5f);
			break;
		case 3:
			ThrowJavelin(-0.5f);
			ThrowJavelin(1f);
			ThrowJavelin(2.5f);
			break;
		case 4:
			ThrowJavelin(0.5f);
			ThrowJavelin(2f);
			ThrowJavelin(3.5f);
			break;
		}
	}

	private void ThrowJavelin(float angleModifier)
	{
		Transform obj = ClosestPlayer();
		Vector3 position = base.transform.position;
		Vector2 vector = obj.position - position;
		Vector2 vector2 = new Vector2(vector.normalized.y, 0f - vector.normalized.x);
		Vector2 vector3 = vector.normalized * 8f * anim.speed + vector2 * angleModifier;
		GameObject gameObject = Object.Instantiate(javelinPrefab, position + (Vector3)(vector.normalized * 0.8f), Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, vector3), Vector3.forward));
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Shoot", position);
		gameObject.GetComponent<Rigidbody2D>().AddForce(vector3, ForceMode2D.Impulse);
		javelinInstances.Add(gameObject);
		Screenshake.Instance.AddTrauma(0.6f);
	}

	private IEnumerator ReturnJavelins()
	{
		GameObject[] array = javelinInstances.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				ReturnJavelin(gameObject);
				Screenshake.Instance.AddTrauma(0.8f);
				yield return new WaitForSeconds(0.2f);
			}
		}
		javelinInstances.Clear();
	}

	private void ReturnJavelin(GameObject javelin)
	{
		Vector2 vector = ClosestPlayer().transform.position - javelin.transform.position;
		javelin.GetComponent<Javelin>().Recall();
		javelin.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		Vector2 vector2 = vector.normalized * 10f * anim.speed;
		javelin.GetComponent<Rigidbody2D>().AddForce(vector2, ForceMode2D.Impulse);
		javelin.transform.rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, vector2), Vector3.forward);
	}
}
