using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DamagedSoundEffect : MonoBehaviour
{
	public EventReference sfx;

	private void Start()
	{
		GetComponent<Health>().OnDamage += PlaySoundEffect;
	}

	public void PlaySoundEffect(float damage, bool causedDeath, GameObject beingAttacked, GameObject attacker)
	{
		RuntimeManager.PlayOneShot(sfx, base.transform.position);
	}
}
