using UnityEngine;

public class EnableOnStart : MonoBehaviour
{
	public GameObject gameObjectToEnable;

	private void Start()
	{
		gameObjectToEnable.SetActive(value: true);
	}

	private void Update()
	{
	}
}
