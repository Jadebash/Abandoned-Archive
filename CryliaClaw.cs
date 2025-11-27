using FMODUnity;
using UnityEngine;

public class CryliaClaw : MonoBehaviour
{
	private float damageTimer;

	private GameObject player;

	private void Start()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/DrawingClaws", base.transform.position);
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
	}

	public void clawPlayer()
	{
		if (Vector2.Distance(base.gameObject.transform.position, player.transform.position) <= 0.5f)
		{
			player.GetComponent<Health>().Damage(20f);
		}
	}

	private void DestroyClaws()
	{
		Object.Destroy(base.gameObject);
	}
}
