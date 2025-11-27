using System.Collections.Generic;
using UnityEngine;

public class GreenLasers : MonoBehaviour
{
	public float rotationSpeed;

	private float currentRotationSpeed;

	private float directionTimer;

	private float destroyTimer;

	private const float switchInterval = 3.75f;

	private const float telegraphDuration = 0.5f;

	private List<SpriteRenderer> laserRenderers = new List<SpriteRenderer>();

	private Color originalColor;

	private void Start()
	{
		directionTimer = 0f;
		destroyTimer = 0f;
		currentRotationSpeed = rotationSpeed;
		foreach (Transform item in base.transform)
		{
			SpriteRenderer component = item.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				laserRenderers.Add(component);
				originalColor = component.color;
			}
		}
	}

	private void Update()
	{
		destroyTimer += Time.deltaTime;
		directionTimer += Time.deltaTime;
		float num = 3.75f - directionTimer;
		if (num <= 0.5f && num > 0f)
		{
			float t = 1f - num / 0.5f;
			currentRotationSpeed = Mathf.Lerp(rotationSpeed, 0f, t);
		}
		else
		{
			currentRotationSpeed = rotationSpeed;
			foreach (SpriteRenderer laserRenderer in laserRenderers)
			{
				laserRenderer.color = originalColor;
			}
		}
		if (directionTimer >= 3.75f)
		{
			directionTimer = 0f;
			if (Random.Range(0, 2) == 0)
			{
				rotationSpeed = 0f - rotationSpeed;
			}
			currentRotationSpeed = rotationSpeed;
		}
		foreach (Transform item in base.transform)
		{
			item.Rotate(Vector3.forward * currentRotationSpeed * Time.deltaTime);
		}
		if (destroyTimer >= 15f)
		{
			GetComponent<Animator>().SetTrigger("Destroy");
		}
	}
}
