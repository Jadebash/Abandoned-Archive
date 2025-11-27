using FMODUnity;
using UnityEngine;

public class CryliaKennel : MonoBehaviour
{
	private float timer;

	private float percent;

	private Vector3 startingPos;

	public float spawnTimer;

	private float maxSpawnTimer;

	public GameObject dog;

	private bool shake;

	private bool audioPlayed;

	private void Start()
	{
		maxSpawnTimer = spawnTimer;
		startingPos = base.transform.position;
	}

	private void Update()
	{
		if (percent >= 1f && !shake)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/CageHittingFloor", base.transform.position);
			Screenshake.Instance.AddTrauma(0.5f);
			shake = true;
		}
		if (percent <= 1f)
		{
			if (!audioPlayed)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/Falling", base.transform.position);
				audioPlayed = true;
			}
			timer += Time.deltaTime;
			percent = timer / 0.4f;
			Vector3 vector = startingPos - new Vector3(0f, 3.75f, 0f);
			base.transform.position = Vector2.Lerp(startingPos, vector, percent);
		}
		else
		{
			spawnTimer -= Time.deltaTime;
			if (spawnTimer <= 0f)
			{
				if (Object.FindObjectsOfType<CryliaDog>().Length < 3)
				{
					Object.Instantiate(dog, base.transform.position - new Vector3(0f, 0.75f, 0f), Quaternion.identity);
				}
				spawnTimer = maxSpawnTimer;
			}
		}
		if (GetComponent<Health>().health <= 0f)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/CageBreaking", base.transform.position);
			Screenshake.Instance.AddTrauma(0.6f);
			Object.Destroy(base.gameObject);
		}
	}
}
