using UnityEngine;

public class DestroyAfterActiveTime : MonoBehaviour
{
	public float time;

	private bool startDestroy;

	private void Start()
	{
		startDestroy = false;
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && !startDestroy)
		{
			startDestroy = true;
		}
		if (startDestroy)
		{
			time -= Time.deltaTime;
			if (time <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
