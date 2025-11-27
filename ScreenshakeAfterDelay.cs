using UnityEngine;

public class ScreenshakeAfterDelay : MonoBehaviour
{
	public float delay = 1f;

	public float shake = 0.7f;

	private void Start()
	{
		Invoke("DoShake", delay);
	}

	private void DoShake()
	{
		Screenshake.Instance.AddTrauma(shake);
	}
}
