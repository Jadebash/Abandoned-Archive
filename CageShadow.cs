using UnityEngine;

public class CageShadow : MonoBehaviour
{
	private GameObject player;

	private Rigidbody2D rb;

	private float fallTimer;

	private float destroyTimer;

	public GameObject cage;

	private bool cageFalling;

	private Vector3 lockPosition;

	private Animator anim;

	private bool destroyed;

	public float speed;

	private void Start()
	{
		anim = GetComponent<Animator>();
		player = PlayerManager.ClosestPlayer(base.gameObject.transform.position);
		rb = GetComponent<Rigidbody2D>();
		fallTimer = 0f;
		cageFalling = false;
		destroyTimer = 0f;
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (!cageFalling)
		{
			fallTimer += Time.deltaTime;
		}
		Vector3 vector = player.transform.position - base.gameObject.transform.position;
		rb.AddForce(vector * speed * Time.deltaTime);
		if (rb.velocity.magnitude >= 4f)
		{
			rb.AddForce(-rb.velocity * speed * Time.deltaTime);
		}
		if (fallTimer >= 6f)
		{
			lockPosition = base.gameObject.transform.position;
			fallTimer = 0f;
			Object.Instantiate(cage, base.gameObject.transform.position + new Vector3(0f, 5f, 0f), Quaternion.identity);
			cageFalling = true;
		}
		if (cageFalling)
		{
			base.transform.position = lockPosition;
			if (!destroyed)
			{
				anim.SetTrigger("Destroy");
				destroyed = true;
			}
		}
	}

	private void DestroyObject()
	{
		Object.Destroy(base.gameObject);
	}
}
