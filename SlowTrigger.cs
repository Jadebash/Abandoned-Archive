using UnityEngine;

public class SlowTrigger : MonoBehaviour
{
	private GameObject player;

	private bool playerSlowed;

	public float slowTimer;

	private float startingTime;

	private void Start()
	{
		startingTime = slowTimer;
		player = PlayerManager.ClosestPlayer(base.transform.position);
	}

	private void Update()
	{
		if (player.GetComponent<Movement>().HasSpeedEffect("SlowTrigger") && !playerSlowed)
		{
			playerSlowed = true;
		}
		if (Vector2.Distance(base.transform.position, player.transform.position) <= 1f && !playerSlowed && !player.GetComponent<Movement>().rolling)
		{
			player.GetComponent<Movement>().AddSpeedEffect(-0.65f, 5f, "SlowTrigger");
			playerSlowed = true;
		}
		if (!playerSlowed)
		{
			return;
		}
		slowTimer -= Time.deltaTime;
		if (slowTimer <= 0f)
		{
			if (player.GetComponent<Movement>().HasSpeedEffect("SlowTrigger"))
			{
				player.GetComponent<Movement>().RemoveSpeedEffect("SlowTrigger");
			}
			playerSlowed = false;
			slowTimer = startingTime;
		}
	}
}
