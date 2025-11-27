using UnityEngine;

public class RoomSinkingAttack : StateMachineBehaviour
{
	private Vector3 startingPos;

	private Vector3 targetPos;

	public float sinkSpeed;

	private bool hasSank;

	public GameObject damageArea;

	private GameObject damageAreaInstance;

	private bool pushingWall;

	public GameObject wisp;

	private GameObject[] wispInstances;

	private float timer;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		startingPos = animator.gameObject.transform.position;
		targetPos = startingPos + Vector3.down * 25f;
		hasSank = false;
		damageAreaInstance = Object.Instantiate(damageArea, startingPos + Vector3.down * 35f, Quaternion.identity);
		GameObject[] players = PlayerManager.players;
		foreach (GameObject obj in players)
		{
			obj.transform.parent = animator.gameObject.transform;
			obj.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
		}
		wispInstances = new GameObject[2];
		wispInstances[0] = Object.Instantiate(wisp, startingPos + new Vector3(10f, 0f, 0f), Quaternion.identity);
		wispInstances[1] = Object.Instantiate(wisp, startingPos + new Vector3(-10f, 0f, 0f), Quaternion.identity);
		timer = 0f;
		pushingWall = false;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timer += Time.deltaTime;
		if (!pushingWall)
		{
			animator.gameObject.transform.position = Vector3.MoveTowards(animator.gameObject.transform.position, targetPos, sinkSpeed * Time.deltaTime);
		}
		GameObject[] players = PlayerManager.players;
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].transform.localPosition.y >= 6f)
			{
				if (!pushingWall)
				{
					pushingWall = true;
				}
				animator.gameObject.transform.position += Vector3.up * sinkSpeed / 2f * Time.deltaTime;
			}
			else if (pushingWall)
			{
				pushingWall = false;
			}
		}
		if ((animator.gameObject.transform.position.y <= targetPos.y || timer >= 10f) && !hasSank)
		{
			hasSank = true;
			targetPos = startingPos + new Vector3(0f, 1.5f, 0f);
		}
		else if (animator.gameObject.transform.position.y >= startingPos.y + 1f)
		{
			animator.SetBool("Sinking", value: false);
			damageAreaInstance.GetComponent<Animator>().SetTrigger("Destroy");
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameObject[] players = PlayerManager.players;
		foreach (GameObject obj in players)
		{
			obj.transform.parent = null;
			obj.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
		}
		players = wispInstances;
		foreach (GameObject gameObject in players)
		{
			if (gameObject != null)
			{
				gameObject.GetComponent<Animator>().SetTrigger("Death");
			}
		}
		Object.Destroy(damageAreaInstance);
	}
}
