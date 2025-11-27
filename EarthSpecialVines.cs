using FMODUnity;
using UnityEngine;

public class EarthSpecialVines : MonoBehaviour
{
	private Enemy[] enemies;

	public float deathTimer;

	private Animator anim;

	private bool destroyed;

	private bool caught;

	private void Start()
	{
		anim = GetComponent<Animator>();
		enemies = Object.FindObjectsOfType<Enemy>();
	}

	private void Update()
	{
		Enemy[] array = enemies;
		foreach (Enemy enemy in array)
		{
			if (enemy != null && Vector2.Distance(enemy.gameObject.transform.position, base.gameObject.transform.position) <= 0.5f && !caught)
			{
				caught = true;
				enemy.Stun(3.5f);
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Vines_Growing", base.transform.position);
			}
		}
		deathTimer -= Time.deltaTime;
		if (deathTimer <= 0f && !destroyed)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Vines_Growing", base.transform.position);
			destroyed = true;
			anim.SetTrigger("Destroy");
		}
	}

	private void DestroyVines()
	{
		Object.Destroy(base.gameObject);
	}
}
