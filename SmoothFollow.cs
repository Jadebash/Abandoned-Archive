using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	public Transform target;

	public float t = 3f;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position = Vector3.Lerp(base.transform.position, target.position, Time.deltaTime * t);
	}
}
