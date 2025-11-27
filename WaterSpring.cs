using UnityEngine;

public class WaterSpring : MonoBehaviour
{
	public float velocity;

	private float force;

	public float height;

	public float targetHeight;

	public GameObject container;

	public bool autoMove;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void WaveSpringUpdate(float springStiffness, float dampening)
	{
		targetHeight = container.transform.position.y;
		height = base.transform.position.y;
		float num = height - targetHeight;
		float num2 = 0f;
		if (!autoMove || velocity >= 0.025f)
		{
			num2 = (0f - dampening) * velocity;
		}
		force = (0f - springStiffness) * num + num2;
		velocity += force;
		float y = base.transform.localPosition.y;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, y + velocity, base.transform.localPosition.z);
	}
}
