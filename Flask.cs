using System.Collections;
using FMODUnity;
using UnityEngine;

public class Flask : MonoBehaviour
{
	private float activateTimer;

	public float speed;

	private bool activated;

	private float explodeTimer = 0.5f;

	public GameObject shatterParticles;

	private void Start()
	{
	}

	private void Update()
	{
		explodeTimer -= Time.deltaTime;
		if (explodeTimer <= 0f && !activated)
		{
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			StartCoroutine(ActivateFlask());
			activated = true;
		}
	}

	private IEnumerator ActivateFlask()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(0.025f);
		}
		yield return new WaitForSeconds(0.75f);
		foreach (Transform item2 in base.transform)
		{
			Vector2 vector = -(base.transform.position - item2.position).normalized;
			item2.GetComponent<Rigidbody2D>().AddForce(vector * speed, ForceMode2D.Impulse);
		}
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/ArmWoosh");
	}

	private void OnDestroy()
	{
		Object.Instantiate(shatterParticles, base.transform.position, Quaternion.identity);
	}
}
