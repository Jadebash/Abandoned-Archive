using FMODUnity;
using UnityEngine;

public class CryliaSyringe : MonoBehaviour
{
	public enum SyringeColour
	{
		RED = 0,
		BLUE = 1,
		BLACK = 2,
		GREEN = 3
	}

	public SyringeColour colour;

	private GameObject player;

	private Crylia crylia;

	private OblomeBoss oblomeBoss;

	private Vector3 playerPos;

	private Rigidbody2D rb;

	private float movementTimer;

	private float moveBackTimer;

	private float destroyTimer;

	private LineRenderer chain;

	private bool audioPlayed;

	private bool moving = true;

	public float speed;

	private void Start()
	{
		if (Object.FindObjectOfType<Crylia>() != null)
		{
			crylia = Object.FindObjectOfType<Crylia>();
			chain = crylia.gameObject.transform.Find("SyringeChain").GetComponent<LineRenderer>();
		}
		else
		{
			oblomeBoss = Object.FindObjectOfType<OblomeBoss>();
			chain = oblomeBoss.gameObject.transform.Find("SyringeChain").GetComponent<LineRenderer>();
		}
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		rb = GetComponent<Rigidbody2D>();
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
		if (crylia != null)
		{
			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), crylia.gameObject.GetComponent<Collider2D>());
		}
		if (oblomeBoss != null)
		{
			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), oblomeBoss.gameObject.GetComponent<Collider2D>());
		}
		CryliaSyringe[] array = Object.FindObjectsOfType<CryliaSyringe>();
		Collider2D component = GetComponent<Collider2D>();
		CryliaSyringe[] array2 = array;
		foreach (CryliaSyringe cryliaSyringe in array2)
		{
			if (cryliaSyringe != this && cryliaSyringe.GetComponent<Collider2D>() != null)
			{
				Physics2D.IgnoreCollision(component, cryliaSyringe.GetComponent<Collider2D>());
			}
		}
		CryliaTrap[] array3 = Object.FindObjectsOfType<CryliaTrap>();
		foreach (CryliaTrap cryliaTrap in array3)
		{
			if (cryliaTrap != null && cryliaTrap.GetComponent<Collider2D>() != null)
			{
				Physics2D.IgnoreCollision(component, cryliaTrap.GetComponent<Collider2D>());
			}
		}
	}

	private void OnEnable()
	{
		Screenshake.Instance.AddTrauma(0.4f);
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		destroyTimer += Time.deltaTime;
		if (destroyTimer >= 2.5f && base.gameObject.activeSelf)
		{
			chain.gameObject.SetActive(value: false);
			Object.Destroy(base.gameObject);
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		chain.gameObject.SetActive(value: true);
		if (crylia != null)
		{
			chain.SetPosition(0, crylia.gameObject.transform.position);
			chain.SetPosition(1, base.transform.position);
		}
		else
		{
			chain.SetPosition(0, oblomeBoss.gameObject.transform.position);
			chain.SetPosition(1, base.transform.position);
		}
		movementTimer += Time.deltaTime;
		if (Vector2.Distance(player.transform.position, base.gameObject.transform.position) <= 1f)
		{
			switch (colour)
			{
			case SyringeColour.RED:
				player.GetComponent<Health>().Damage(20f);
				break;
			case SyringeColour.BLUE:
				if (!crylia.slowedPlayer)
				{
					player.GetComponent<Movement>().AddSpeedEffect(-0.5f, 5f, "CryliaSyringe");
					crylia.slowedPlayer = true;
				}
				break;
			case SyringeColour.BLACK:
				crylia.BlindPlayer();
				break;
			case SyringeColour.GREEN:
				crylia.GreenSyringeEffect();
				break;
			default:
				Debug.Log("Invalid Syringe");
				break;
			}
		}
		if (moving)
		{
			rb.AddForce(base.transform.up.normalized * speed * Time.deltaTime);
			return;
		}
		if (!audioPlayed)
		{
			Screenshake.Instance.AddTrauma(0.4f);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ChainWoosh", base.transform.position);
			audioPlayed = true;
		}
		if (crylia != null)
		{
			if (Vector2.Distance(crylia.gameObject.transform.position, base.gameObject.transform.position) >= 2f)
			{
				Vector3 vector = crylia.gameObject.transform.position - base.gameObject.transform.position;
				rb.AddForce(vector * 450f * Time.deltaTime);
				Vector3 vector2 = (player.transform.position - base.gameObject.transform.position) * 0.4f;
				rb.AddForce(vector2 * 450f * Time.deltaTime);
			}
			else
			{
				chain.gameObject.SetActive(value: false);
				Object.Destroy(base.gameObject);
			}
		}
		else if (Vector2.Distance(oblomeBoss.gameObject.transform.position, base.gameObject.transform.position) >= 2f)
		{
			Vector3 vector3 = oblomeBoss.gameObject.transform.position - base.gameObject.transform.position;
			rb.AddForce(vector3 * 450f * Time.deltaTime);
			Vector3 vector4 = (player.transform.position - base.gameObject.transform.position) * 0.4f;
			rb.AddForce(vector4 * 450f * Time.deltaTime);
		}
		else
		{
			chain.gameObject.SetActive(value: false);
			Object.Destroy(base.gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		rb.velocity = new Vector2(0f, 0f);
		moving = false;
	}
}
