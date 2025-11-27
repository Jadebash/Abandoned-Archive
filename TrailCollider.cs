using UnityEngine;

public class TrailCollider : MonoBehaviour
{
	public GameObject prefab;

	public float timeBetweenSpawns;

	private float timer;

	public TrailRenderer trailRenderer;

	public Rigidbody2D rb;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > timeBetweenSpawns)
		{
			timer -= timeBetweenSpawns;
			if (rb.simulated)
			{
				Object.Destroy(Object.Instantiate(prefab, base.transform.position, Quaternion.identity), trailRenderer.time - 0.2f);
			}
		}
	}
}
