using UnityEngine;

public class StopAfterTime : MonoBehaviour
{
	public float time;

	private float timer;

	public Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > time && rb.simulated)
		{
			rb.simulated = false;
		}
	}
}
