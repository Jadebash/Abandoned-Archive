using FMODUnity;
using UnityEngine;

public class EarthMiniVines : MonoBehaviour
{
	private float deathTimer = 4f;

	private Animator anim;

	private bool destroyed;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	private void Update()
	{
		deathTimer -= Time.deltaTime;
		if (deathTimer <= 0f && !destroyed)
		{
			destroyed = true;
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Vines_Growing", base.transform.position);
			anim.SetTrigger("Destroy");
		}
	}

	private void DestroyMiniVines()
	{
		Object.Destroy(base.gameObject);
	}
}
