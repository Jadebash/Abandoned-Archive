using UnityEngine;

public class TrapShadow : MonoBehaviour
{
	private GameObject player;

	private Rigidbody2D rb;

	private float fallTimer;

	private float destroyTimer;

	public GameObject trap;

	private bool trapFalling;

	private Vector3 lockPosition;

	private Vector3 startingPosition;

	private Vector3 targetPosition;

	private float randX;

	private float randY;

	private float timeToReachTarget = 3f;

	private void Start()
	{
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		rb = GetComponent<Rigidbody2D>();
		fallTimer = 0f;
		trapFalling = false;
		destroyTimer = 0f;
		startingPosition = base.transform.position;
		randX = Random.Range(-3f, 3f);
		randY = Random.Range(-3f, 3f);
		targetPosition = startingPosition + new Vector3(randX, randY, 0f);
	}

	private void Update()
	{
		if (!trapFalling)
		{
			fallTimer += Time.deltaTime;
			float num = timeToReachTarget - fallTimer;
			if (num > 0.01f)
			{
				Vector3 position = base.transform.position;
				Vector3 vector = (targetPosition - position) / num;
				if (vector.magnitude > 4f)
				{
					vector = vector.normalized * 4f;
				}
				rb.velocity = vector;
			}
			else
			{
				base.transform.position = targetPosition;
				rb.velocity = Vector2.zero;
			}
		}
		if (fallTimer >= timeToReachTarget)
		{
			lockPosition = targetPosition;
			fallTimer = 0f;
			Object.Instantiate(trap, lockPosition + new Vector3(0f, 5f, 0f), Quaternion.identity);
			trapFalling = true;
			rb.velocity = Vector2.zero;
		}
		if (trapFalling)
		{
			base.transform.position = lockPosition;
			destroyTimer += Time.deltaTime;
			if (destroyTimer >= 0.5f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
