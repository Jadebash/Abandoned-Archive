using FMODUnity;
using UnityEngine;

public class EmeraldCircle : MonoBehaviour
{
	private float timer;

	private bool moving;

	private float destroyTimer;

	private void Start()
	{
		timer = 0f;
		moving = false;
		destroyTimer = 0f;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= 2f && !moving && Time.timeScale != 0f)
		{
			moving = true;
			foreach (Transform item in base.transform)
			{
				if (item.gameObject.GetComponent<Rigidbody2D>() != null)
				{
					Vector3 vector = base.transform.position - item.position;
					item.gameObject.GetComponent<Rigidbody2D>().AddForce(vector * 2.5f, ForceMode2D.Impulse);
				}
			}
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/RodhCrystalThrow");
		}
		if (moving && Time.timeScale != 0f)
		{
			destroyTimer += Time.deltaTime;
			if (destroyTimer >= 0.5f)
			{
				GetComponent<Animator>().SetTrigger("Death");
			}
		}
	}

	private void Death()
	{
		Object.Destroy(base.gameObject);
	}
}
