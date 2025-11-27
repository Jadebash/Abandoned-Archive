using FMODUnity;
using UnityEngine;

public class RufusFloorboard : MonoBehaviour
{
	private float percent;

	private float timer;

	private Vector3 startingPos;

	private Vector3 targetPos;

	public bool onFire;

	private float deathTimer = 5f;

	private bool sePlayed;

	private bool fallingSoundPlayed;

	private float fallTimer;

	private void Start()
	{
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Object.FindObjectOfType<Rufus>().GetComponent<Collider2D>());
		startingPos = base.transform.position;
		targetPos = startingPos + new Vector3(0f, -7f, 0f);
		fallTimer = Random.Range(0.6f, 1.25f);
	}

	private void Update()
	{
		if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FloorboardDeath"))
		{
			return;
		}
		timer += Time.deltaTime;
		percent = timer / fallTimer;
		if (percent <= 1f)
		{
			base.transform.position = Vector2.Lerp(startingPos, targetPos, percent);
		}
		else if (!fallingSoundPlayed)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/WoodHittingFloor", base.transform.position);
			Screenshake.Instance.AddTrauma(0.3f);
			fallingSoundPlayed = true;
		}
		if (onFire)
		{
			if (!sePlayed)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/WoodBurning", base.transform.position);
				sePlayed = true;
			}
			deathTimer -= Time.deltaTime;
			if (deathTimer <= 0f)
			{
				GetComponent<Animator>().SetTrigger("Death");
			}
			GameObject gameObject = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
			if (gameObject != null && Vector2.Distance(gameObject.transform.position, base.gameObject.transform.position) <= 1f)
			{
				gameObject.GetComponent<Health>().Damage(20f);
				GetComponent<Animator>().SetTrigger("Death");
			}
		}
	}

	private void OnParticleCollision(GameObject particle)
	{
		if (particle.gameObject.name == "FlameParticles")
		{
			onFire = true;
			base.transform.Find("Particles").gameObject.SetActive(value: true);
		}
	}

	public void Death()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/WoodBreaking", base.transform.position);
		Object.Destroy(base.gameObject);
	}
}
