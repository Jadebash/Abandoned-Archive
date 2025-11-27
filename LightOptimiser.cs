using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightOptimiser : MonoBehaviour
{
	private Transform[] players;

	private Light2D light2D;

	private void Start()
	{
		light2D = GetComponent<Light2D>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		players = new Transform[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			players[i] = array[i].transform;
		}
		if (players.Length != 0)
		{
			UpdateLight();
			InvokeRepeating("UpdateLight", Random.Range(0f, 1f), 1f);
		}
		else
		{
			Debug.LogWarning("No players found!");
		}
	}

	private void OnEnable()
	{
		if (players != null && players.Length != 0)
		{
			UpdateLight();
		}
	}

	public void UpdateLight()
	{
		if ((MapGenerator.Instance != null && MapGenerator.Instance.dontOptimise) || !base.gameObject.activeInHierarchy || players == null || players.Length == 0 || !base.gameObject.activeSelf)
		{
			return;
		}
		bool flag = false;
		Transform[] array = players;
		foreach (Transform transform in array)
		{
			if (transform != null && Vector3.Distance(transform.position, base.transform.position) < light2D.pointLightOuterRadius + 12f)
			{
				flag = true;
				break;
			}
		}
		light2D.enabled = flag;
	}
}
