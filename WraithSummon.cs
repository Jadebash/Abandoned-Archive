using System.Collections;
using FMODUnity;
using UnityEngine;

public class WraithSummon : MonoBehaviour
{
	private LostApparition lost;

	private GameObject player;

	private Rigidbody2D rb;

	private float resetTimer = 2.5f;

	private Vector3 startingPos;

	private float moveTimer;

	private float startingMoveTimer;

	private bool reset;

	private float timer;

	private bool screenshaken;

	private Animator anim;

	private bool faded;

	private void Start()
	{
		anim = GetComponent<Animator>();
		moveTimer = Random.Range(0.75f, 1.25f);
		startingMoveTimer = moveTimer;
		rb = GetComponent<Rigidbody2D>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		lost = Object.FindObjectOfType<LostApparition>();
		startingPos = GetValidSpawnPosition();
		base.transform.position = startingPos;
		StartCoroutine(delayedSoundEffect());
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (player.transform.position.x < base.transform.position.x && !GetComponent<SpriteRenderer>().flipX)
		{
			GetComponent<SpriteRenderer>().flipX = true;
		}
		else if (player.transform.position.x > base.transform.position.x && GetComponent<SpriteRenderer>().flipX)
		{
			GetComponent<SpriteRenderer>().flipX = false;
		}
		resetTimer -= Time.deltaTime;
		moveTimer -= Time.deltaTime;
		if (resetTimer <= 0.625f && !faded)
		{
			anim.SetTrigger("Fade");
			faded = true;
		}
		if (resetTimer <= 0f)
		{
			rb.AddForce(-rb.velocity, ForceMode2D.Impulse);
			startingPos = GetValidSpawnPosition();
			base.transform.position = startingPos;
			resetTimer = 2.5f;
			moveTimer = startingMoveTimer;
			reset = true;
			faded = false;
		}
		if (reset)
		{
			base.transform.position = startingPos;
			timer += Time.deltaTime;
			if (timer >= 1f)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/GhostWoosh", lost.gameObject.transform.position);
				reset = false;
				screenshaken = false;
				resetTimer = 2.5f;
			}
		}
		if (moveTimer <= 0f && !reset)
		{
			Vector3 vector = player.transform.position - base.gameObject.transform.position;
			rb.AddForce(vector * 150f * Time.deltaTime);
		}
		if (Vector2.Distance(player.transform.position, base.transform.position) <= 2.5f && !screenshaken)
		{
			Screenshake.Instance.AddTrauma(0.5f);
			screenshaken = true;
		}
	}

	public void forceDestroy()
	{
		Object.Destroy(base.gameObject);
	}

	public void isDying()
	{
		anim.SetBool("Dying", value: true);
	}

	private IEnumerator delayedSoundEffect()
	{
		yield return new WaitForSeconds(1f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/GhostWoosh", lost.gameObject.transform.position);
	}

	private Vector3 GetValidSpawnPosition()
	{
		bool flag = false;
		Vector3 vector = Vector3.zero;
		int num = 0;
		while (!flag && num < 50)
		{
			num++;
			float x = Random.Range(-3f, 3f);
			float y = Random.Range(-3f, 3f);
			vector = lost.gameObject.transform.position + new Vector3(x, y, 0f);
			flag = true;
			GameObject[] players = PlayerManager.players;
			foreach (GameObject gameObject in players)
			{
				if (Vector3.Distance(vector, gameObject.transform.position) < 2.5f)
				{
					flag = false;
					break;
				}
			}
		}
		return vector;
	}
}
