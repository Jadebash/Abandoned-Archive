using FMODUnity;
using UnityEngine;

public class CryliaNails : MonoBehaviour
{
	public float speed;

	private Rigidbody2D rb;

	private Crylia crylia;

	private float movementTimer;

	private bool sePlayed;

	public GameObject trail;

	public GameObject slowTrigger;

	private GameObject trailInstance;

	private void Start()
	{
		movementTimer = Random.Range(0.25f, 0.75f);
		crylia = Object.FindObjectOfType<Crylia>();
		CryliaNails[] array = Object.FindObjectsOfType<CryliaNails>();
		for (int i = 0; i < array.Length; i++)
		{
			Physics2D.IgnoreCollision(array[i].gameObject.GetComponent<Collider2D>(), crylia.gameObject.GetComponent<Collider2D>());
			Physics2D.IgnoreCollision(array[i].gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		}
		rb = GetComponent<Rigidbody2D>();
		trailInstance = Object.Instantiate(trail, base.transform.position, Quaternion.identity);
		trailInstance.GetComponent<LineRenderer>().SetPosition(0, crylia.gameObject.transform.position);
		trailInstance.GetComponent<LineRenderer>().SetPosition(1, crylia.gameObject.transform.position);
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		trailInstance.GetComponent<LineRenderer>().SetPosition(1, base.transform.position);
		movementTimer -= Time.deltaTime;
		if (movementTimer <= 0f)
		{
			rb.AddForce(base.transform.up * speed * Time.deltaTime);
			if (!sePlayed)
			{
				RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ThrowNails", base.transform.position);
				sePlayed = true;
			}
		}
	}

	private void OnCollisionEnter2D()
	{
		if (trailInstance != null)
		{
			LineRenderer component = trailInstance.GetComponent<LineRenderer>();
			if (component != null)
			{
				Vector3 position = component.GetPosition(0);
				Vector3 position2 = component.GetPosition(1);
				for (int i = 1; i <= 6; i++)
				{
					float t = (float)i / 7f;
					Vector3 position3 = Vector3.Lerp(position, position2, t);
					Object.Instantiate(slowTrigger, position3, Quaternion.identity);
				}
			}
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/NailDestroy", base.transform.position);
		Object.Destroy(base.gameObject);
		Screenshake.Instance.AddTrauma(0.5f);
	}
}
