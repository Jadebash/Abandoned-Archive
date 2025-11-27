using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Flicker : MonoBehaviour
{
	public float speed = 1f;

	public float amplitude = 0.1f;

	private float seed;

	private Light2D light;

	private float startingIntensity;

	private void Start()
	{
		light = GetComponent<Light2D>();
		startingIntensity = light.intensity;
		seed = Random.Range(-10000f, 10000f);
	}

	private void Update()
	{
		light.intensity = startingIntensity + (Mathf.PerlinNoise(Time.time * speed, seed) - 0.5f) * 2f * amplitude * startingIntensity;
	}
}
