using FMODUnity;
using UnityEngine;

public class Javelin : MonoBehaviour
{
	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	public void Recall()
	{
		rb.simulated = true;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if ((bool)other.transform.GetComponent<Boss>() || (bool)other.transform.GetComponent<Javelin>())
		{
			Debug.Log("Javelin hit a boss or another javelin");
			return;
		}
		Screenshake.Instance.AddTrauma(0.5f);
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/JavelinHittingWall", base.transform.position);
		rb.simulated = false;
	}
}
