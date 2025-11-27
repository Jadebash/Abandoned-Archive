using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
	private Health health;

	private Animator respawnAnim;

	private void Start()
	{
		health = GetComponent<Health>();
		health.OnDeath += Health_OnDeath;
		respawnAnim = GetComponent<Animator>();
	}

	public void Health_OnDeath(GameObject attacker)
	{
		if (Death.Instance.PlayerDeath(attacker, health))
		{
			respawnAnim.SetTrigger("Die");
		}
		else
		{
			health.ResetHasDied();
		}
	}

	public void FinishDeath()
	{
		Death.Instance.FinishDeath();
	}
}
