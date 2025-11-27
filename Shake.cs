using UnityEngine;

public class Shake : MonoBehaviour
{
	public bool doShake;

	public bool useLocalPosition;

	public float speed = 7.5f;

	public float amplitude = 100f;

	private float randomX;

	private float randomY;

	private Vector3 startingPosition;

	private void Start()
	{
		startingPosition = base.transform.position;
		if (useLocalPosition)
		{
			startingPosition = base.transform.localPosition;
		}
		randomX = Random.Range(0f, 10000f);
		randomY = Random.Range(0f, 10000f);
	}

	private void Update()
	{
		if (doShake)
		{
			float num = (Mathf.PerlinNoise(randomX, Time.realtimeSinceStartup * speed) - 0.5f) * amplitude;
			float num2 = (Mathf.PerlinNoise(randomY, Time.realtimeSinceStartup * speed) - 0.5f) * amplitude;
			if (useLocalPosition)
			{
				base.transform.localPosition = new Vector3(startingPosition.x + num, startingPosition.y + num2, startingPosition.z);
			}
			else
			{
				base.transform.position = new Vector3(startingPosition.x + num, startingPosition.y + num2, startingPosition.z);
			}
		}
	}
}
