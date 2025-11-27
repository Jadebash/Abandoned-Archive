using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class LostApparition : MonoBehaviour
{
	public int numOfAttacks;

	public GameObject ghostBall;

	public GameObject wraith;

	public GameObject spear;

	public GameObject spearProjectile;

	[HideInInspector]
	public Vector3 startingPos;

	private float invertedTimer = 10f;

	public GameObject haze;

	public List<SpriteRenderer> sprites;

	private bool flipped;

	private AttackHelper attackHelper;

	private void Start()
	{
		startingPos = base.gameObject.transform.position;
		attackHelper = GetComponent<AttackHelper>();
	}

	private void Update()
	{
		if (GetComponent<Health>().health <= 0f && PlayerManager.ClosestPlayer(base.gameObject.transform.position).GetComponent<Movement>().inverted)
		{
			PlayerManager.ClosestPlayer(base.gameObject.transform.position).GetComponent<Movement>().inverted = false;
		}
		Transform transform = PlayerManager.ClosestPlayer(base.transform.position).transform;
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

	private void SummonGhostBalls()
	{
		Object.Instantiate(ghostBall, base.transform.position, Quaternion.identity);
	}

	private void SummonWraiths()
	{
		for (int i = 0; i < 4; i++)
		{
			Object.Instantiate(wraith);
		}
	}

	private IEnumerator SummonSpears()
	{
		GameObject player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		Object.Instantiate(haze, base.transform.position, Quaternion.identity);
		player.GetComponent<Movement>().AddSpeedEffect(-0.5f, 6.55f, "LostApparition");
		Vector3 offset = new Vector3(0f, -0.5f, 0f);
		yield return new WaitForSeconds(1.5f);
		Object.Instantiate(spear, attackHelper.predictedPlayerPosition(player, 1f, isClamped: true, offset), Quaternion.identity);
		yield return new WaitForSeconds(0.2f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Spear", base.transform.position);
		yield return new WaitForSeconds(1.5f);
		Object.Instantiate(spear, attackHelper.predictedPlayerPosition(player, 1f, isClamped: true, offset), Quaternion.identity);
		yield return new WaitForSeconds(0.2f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Spear", base.transform.position);
		yield return new WaitForSeconds(1.5f);
		Object.Instantiate(spear, attackHelper.predictedPlayerPosition(player, 1f, isClamped: true, offset), Quaternion.identity);
		yield return new WaitForSeconds(0.2f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Spear", base.transform.position);
		yield return new WaitForSeconds(1.5f);
		Object.Instantiate(spear, attackHelper.predictedPlayerPosition(player, 1f, isClamped: true, offset), Quaternion.identity);
		yield return new WaitForSeconds(0.2f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Spear", base.transform.position);
		yield return new WaitForSeconds(0.75f);
		player.GetComponent<Movement>().RemoveSpeedEffect("LostApparition");
	}

	private void ResetPosition()
	{
		base.transform.position = startingPos;
	}
}
