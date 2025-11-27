using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public float time = 4f;

	private float timer;

	private void Start()
	{
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= time)
		{
			DestroyThis();
		}
	}

	public void DestroyThis()
	{
		Object.Destroy(base.gameObject);
	}
}
