using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SirVeltrine : MonoBehaviour
{
	private Transform player;

	private Animator anim;

	public GameObject coinPrefab;

	public GameObject smokePrefab;

	public GameObject chosenCardPrefab;

	public GameObject enemyPrefab;

	public GameObject groundFirePrefab;

	public List<GameObject> cards;

	private Rigidbody2D rb;

	private Vector3 startingPosition;

	[HideInInspector]
	public bool doingCardAttack;

	public LayerMask firePhysicsOverlapLayerMask;

	public LayerMask teleportLayerMask;

	public LayerMask validTeleportLayerMask;

	private Collider2D bossRoomBounds;

	private bool flipped;

	private bool inPhaseTwo;

	public Transform coinSpawn;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		startingPosition = base.transform.position;
		anim = GetComponent<Animator>();
		Collider2D[] array = Physics2D.OverlapPointAll(base.transform.position);
		foreach (Collider2D collider2D in array)
		{
			if (collider2D.CompareTag("PlayableSpace"))
			{
				bossRoomBounds = collider2D;
				break;
			}
		}
	}

	public void ThrowCoin(float angleModifier)
	{
		Vector3 position = base.transform.position;
		player = PlayerManager.ClosestPlayer(base.transform.position).transform;
		Vector2 vector = player.position - position;
		SpriteRenderer component = base.gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		if (vector.x < 0f && !component.flipX)
		{
			flipped = true;
			component.flipX = true;
		}
		else if (vector.x > 0f && component.flipX)
		{
			flipped = false;
			component.flipX = false;
		}
		if (component.flipX)
		{
			coinSpawn.localPosition = new Vector3(-0.44f, 0.4f, 0f);
		}
		else
		{
			coinSpawn.localPosition = new Vector3(0.44f, 0.4f, 0f);
		}
		Vector2 vector2 = new Vector2(vector.normalized.y, 0f - vector.normalized.x);
		GameObject obj = Object.Instantiate(coinPrefab, coinSpawn.position, Quaternion.identity);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Shoot", position);
		obj.GetComponent<Rigidbody2D>().AddForce(vector.normalized * 5f * anim.speed + vector2 * angleModifier * 2f, ForceMode2D.Impulse);
		Screenshake.Instance.AddTrauma(0.3f);
	}

	public void TeleportBack()
	{
		Object.Instantiate(smokePrefab, base.transform.position, Quaternion.identity);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/poof", base.transform.position);
		rb.position = startingPosition;
	}

	public void Teleport()
	{
		if (bossRoomBounds == null)
		{
			Debug.LogWarning("SirVeltrine has no bossRoomBounds assigned.");
			return;
		}
		float num = 7f;
		float num2 = 5f;
		int num3 = 0;
		Vector2 vector2;
		while (true)
		{
			Vector2 vector = (player ? player.position : startingPosition);
			vector2 = vector + Random.insideUnitCircle * num;
			num3++;
			if (bossRoomBounds.OverlapPoint(vector2) && Physics2D.OverlapCircleAll(vector2, 0.25f, teleportLayerMask).Length == 0 && Physics2D.OverlapCircleAll(vector2, 0.1f, validTeleportLayerMask).Length != 0 && Vector2.Distance(vector2, vector) > 3.5f && Mathf.Abs(vector2.y - startingPosition.y) <= num2)
			{
				break;
			}
			if (num3 >= 100)
			{
				Debug.LogWarning("Veltrine could not find a teleport position inside boss room.");
				return;
			}
		}
		Object.Instantiate(smokePrefab, base.transform.position, Quaternion.identity);
		rb.position = vector2;
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/poof", base.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
	}

	public void ShuffleCards()
	{
		foreach (GameObject card in cards)
		{
			card.GetComponent<SirVeltrinePlayingCard>().cardType = (SirVeltrineCardType)Random.Range(0, 5);
			if (card.GetComponent<SirVeltrinePlayingCard>().cardType == SirVeltrineCardType.HealPlayer && Random.Range(0f, 100f) < 50f)
			{
				card.GetComponent<SirVeltrinePlayingCard>().cardType = (SirVeltrineCardType)Random.Range(0, 5);
			}
		}
	}

	public void ChooseCard(SirVeltrinePlayingCard card)
	{
		if (doingCardAttack)
		{
			GameObject gameObject = Object.Instantiate(chosenCardPrefab, card.transform.position, Quaternion.identity, base.transform);
			gameObject.GetComponent<SirVeltrineCardTypeBehaviour>().EnableIcons(card.cardType);
			gameObject.transform.LeanMove(base.transform.position - new Vector3(0f, 1.5f, 0f), 0.5f);
			doingCardAttack = false;
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/CardSliding", base.transform.position);
			anim.SetTrigger("CardChosen");
			StartCoroutine(CardAttack(card.cardType, gameObject.transform));
		}
	}

	public IEnumerator CardAttack(SirVeltrineCardType type, Transform card)
	{
		yield return new WaitForSeconds(2f);
		switch (type)
		{
		case SirVeltrineCardType.Coins:
			ThrowCoin(-3f);
			ThrowCoin(3f);
			yield return new WaitForSeconds(0.1f);
			ThrowCoin(-2f);
			ThrowCoin(2f);
			yield return new WaitForSeconds(0.1f);
			ThrowCoin(-1f);
			ThrowCoin(1f);
			yield return new WaitForSeconds(0.1f);
			ThrowCoin(0f);
			yield return new WaitForSeconds(0.1f);
			ThrowCoin(-1f);
			ThrowCoin(1f);
			yield return new WaitForSeconds(0.1f);
			ThrowCoin(-2f);
			ThrowCoin(2f);
			yield return new WaitForSeconds(0.1f);
			ThrowCoin(-3f);
			ThrowCoin(3f);
			break;
		case SirVeltrineCardType.Fire:
		{
			List<Vector3> existingFirePositions = new List<Vector3>();
			for (int i = 0; i < 3; i++)
			{
				Vector2 vector = (Vector2)player.position + Random.insideUnitCircle * 3f;
				int num = 0;
				while (Physics2D.OverlapCircleAll(vector, 1.28f, firePhysicsOverlapLayerMask).Length != 0 || IsTooClose(vector, 0.7f, existingFirePositions))
				{
					vector = (Vector2)player.position + Random.insideUnitCircle * 3f;
					num++;
					if (num > 100)
					{
						Debug.LogWarning("Could not find valid fire position.");
					}
				}
				Object.Instantiate(groundFirePrefab, vector, Quaternion.identity);
				existingFirePositions.Add(vector);
				Screenshake.Instance.AddTrauma(0.5f);
				yield return new WaitForSeconds(0.2f);
			}
			break;
		}
		case SirVeltrineCardType.HealVeltrine:
			GetComponent<Health>().Heal(20f);
			break;
		case SirVeltrineCardType.HealPlayer:
			RunManager.Instance.SpawnHealthPickup(card.position);
			break;
		case SirVeltrineCardType.SpawnEnemy:
			Object.Instantiate(enemyPrefab, card.position, Quaternion.identity);
			break;
		}
	}

	private IEnumerator ContinuousFireDuringCardAttack()
	{
		float fireRate = 1f;
		float minFireRate = 0.2f;
		float fireAcceleration = 0.725f;
		if (inPhaseTwo)
		{
			fireRate = 0.8f;
			minFireRate = 0.15f;
			fireAcceleration = 0.725f;
		}
		while (doingCardAttack)
		{
			anim.SetTrigger("OneShotCoin");
			if (fireRate < 0.3f)
			{
				anim.SetFloat("OneShotSpeed", 0.3f / fireRate);
			}
			yield return new WaitForSeconds(fireRate);
			fireRate *= fireAcceleration;
			fireRate = Mathf.Max(fireRate, minFireRate);
		}
		anim.SetFloat("OneShotSpeed", 1f);
	}

	private bool IsTooClose(Vector3 position, float radius, List<Vector3> positions)
	{
		foreach (Vector3 position2 in positions)
		{
			if (Vector3.Distance(position2, position) < radius)
			{
				return true;
			}
		}
		return false;
	}

	public void AllCards()
	{
		if (!doingCardAttack)
		{
			return;
		}
		doingCardAttack = false;
		foreach (GameObject card in cards)
		{
			GameObject gameObject = Object.Instantiate(chosenCardPrefab, card.transform.position, Quaternion.identity, base.transform);
			gameObject.GetComponent<SirVeltrineCardTypeBehaviour>().EnableIcons(card.GetComponent<SirVeltrinePlayingCard>().cardType);
			gameObject.transform.LeanMove(card.transform.position + new Vector3(0f, 1.5f, 0f), 0.5f);
			anim.SetTrigger("CardChosen");
			StartCoroutine(CardAttack(card.GetComponent<SirVeltrinePlayingCard>().cardType, gameObject.transform));
		}
	}

	public void DoingCardAttack()
	{
		doingCardAttack = true;
		StartCoroutine(ContinuousFireDuringCardAttack());
	}

	public void StartPhaseTwo()
	{
		doingCardAttack = false;
		StopAllCoroutines();
		anim.ResetTrigger("DealCards");
		anim.ResetTrigger("CardChosen");
		GameObject[] array = GameObject.FindGameObjectsWithTag("Bullet");
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
		array = GameObject.FindGameObjectsWithTag("BossAttack");
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2 != null)
			{
				gameObject2.SetActive(value: false);
			}
		}
		array = GameObject.FindGameObjectsWithTag("ChosenCard");
		foreach (GameObject obj in array)
		{
			obj.GetComponent<SirVeltrineCardTypeBehaviour>().DisableCard();
			Object.Destroy(obj, 0.5f);
		}
		Boss component = GetComponent<Boss>();
		if (component != null)
		{
			component.StartPhaseTwo();
		}
		inPhaseTwo = true;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
