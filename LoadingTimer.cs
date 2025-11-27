using UnityEngine;

public class LoadingTimer : MonoBehaviour
{
	private float timer;

	private void Start()
	{
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && (bool)Object.FindObjectOfType<Health>())
		{
			timer += Time.deltaTime;
			if (timer >= 10f)
			{
				Debug.Log("Loading timer finished");
				base.gameObject.SetActive(value: false);
				timer = 0f;
			}
		}
	}
}
