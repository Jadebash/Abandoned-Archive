using System.Collections;
using FMODUnity;
using UnityEngine;

public class CirclePlayer : StateMachineBehaviour
{
	private Vector3 startingPos;

	private Vector3 targetPos;

	private Vector3 finalPos;

	private GameObject lost;

	private float percent;

	private float timer;

	private float returnPercent;

	private float returnTimer;

	private float angle;

	private float radius = 9f;

	private float speed = 3f;

	private bool circling = true;

	private GameObject spear;

	private GameObject player;

	public float amountOfSpears;

	private float soundTimer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		lost = animator.gameObject;
		spear = lost.GetComponent<LostApparition>().spearProjectile;
		startingPos = lost.transform.position;
		targetPos = startingPos + new Vector3(5f, 0f, 0f);
		CoroutineEmitter.Instance.StartCoroutine(throwSpears());
		soundTimer = 1.5f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (circling)
		{
			soundTimer -= Time.deltaTime;
			if (soundTimer <= 0f)
			{
				soundTimer = 2f;
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/GhostWoosh", animator.gameObject.transform.position);
			}
			float x = startingPos.x + Mathf.Cos(angle) * radius;
			float y = startingPos.y + Mathf.Sin(angle) * radius;
			float z = startingPos.z;
			Vector3 vector = new Vector3(x, y, z);
			angle += speed * Time.deltaTime;
			timer += Time.deltaTime;
			percent = timer / 13f;
			if (percent <= 1f)
			{
				lost.transform.position = Vector2.Lerp(startingPos, vector, percent);
				return;
			}
			circling = false;
			finalPos = lost.transform.position;
		}
		else
		{
			returnTimer += Time.deltaTime;
			returnPercent = returnTimer / 3f;
			if (returnPercent <= 1f)
			{
				lost.transform.position = Vector2.Lerp(finalPos, startingPos, returnPercent);
			}
			else
			{
				animator.SetBool("CirclingPlayer", value: false);
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		circling = true;
		percent = 0f;
		timer = 0f;
		returnPercent = 0f;
		returnTimer = 0f;
	}

	private IEnumerator throwSpears()
	{
		yield return new WaitForSeconds(4.5f);
		for (int i = 0; (float)i < amountOfSpears; i++)
		{
			Object.Instantiate(spear, lost.transform.position, Quaternion.identity);
			yield return new WaitForSeconds(0.25f);
		}
	}
}
