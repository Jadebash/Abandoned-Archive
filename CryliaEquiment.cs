using FMODUnity;
using UnityEngine;

public class CryliaEquiment : MonoBehaviour
{
	private Rigidbody2D rb;

	private Transform playerPos;

	private Vector3 dir;

	private Crylia crylia;

	private void Start()
	{
		crylia = Object.FindObjectOfType<Crylia>();
		rb = GetComponent<Rigidbody2D>();
		playerPos = PlayerManager.ClosestPlayer(base.gameObject.transform.position).transform;
		dir = crylia.CalculateThrowTarget(base.gameObject.transform.position) - base.gameObject.transform.position;
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerPos.gameObject.GetComponent<Collider2D>());
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), crylia.gameObject.GetComponent<Collider2D>());
		CryliaTrap[] array = Object.FindObjectsOfType<CryliaTrap>();
		Collider2D component = GetComponent<Collider2D>();
		CryliaTrap[] array2 = array;
		foreach (CryliaTrap cryliaTrap in array2)
		{
			if (cryliaTrap.GetComponent<Collider2D>() != null)
			{
				Physics2D.IgnoreCollision(component, cryliaTrap.GetComponent<Collider2D>());
			}
		}
	}

	private void FixedUpdate()
	{
		if (Time.timeScale != 0f)
		{
			rb.AddForce(dir * 5.5f);
		}
	}

	private void OnCollisionEnter2D()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/EquipmentBreaking", base.transform.position);
		Screenshake.Instance.AddTrauma(0.6f);
		Object.Destroy(base.gameObject);
	}
}
