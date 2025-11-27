using UnityEngine;

public class CoroutineEmitter : MonoBehaviour
{
	public static CoroutineEmitter Instance;

	private void Start()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
