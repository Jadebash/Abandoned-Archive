using UnityEngine;

public class WaterDamage : MonoBehaviour
{
	private GameObject player;

	private bool slowed;

	private void Start()
	{
		player = PlayerManager.ClosestPlayer(base.transform.position);
	}

	private void Update()
	{
		if (player == null)
		{
			return;
		}
		if (player.transform.position.y < base.transform.position.y)
		{
			if (!slowed)
			{
				player.GetComponent<Movement>().speedModifier -= 0.5f;
				slowed = true;
			}
		}
		else if (slowed)
		{
			player.GetComponent<Movement>().speedModifier += 0.5f;
			slowed = false;
		}
	}

	private void OnDestroy()
	{
		if (player != null && slowed)
		{
			player.GetComponent<Movement>().speedModifier += 0.5f;
		}
	}

	public void Death()
	{
		Object.Destroy(base.gameObject);
	}
}
