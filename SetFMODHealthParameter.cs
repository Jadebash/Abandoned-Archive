using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class SetFMODHealthParameter : MonoBehaviour
{
	private Health health;

	private void Start()
	{
		health = GetComponent<Health>();
		health.OnDamage += OnDamage;
	}

	public void OnDamage(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		RuntimeManager.StudioSystem.setParameterByName("Health", Mathf.RoundToInt(health.health / 20f));
	}
}
