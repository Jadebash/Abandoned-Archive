using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomOptimiser : MonoBehaviour
{
	private Transform player;

	private void Awake()
	{
		if (SpellCasting.Instances.Count != 0)
		{
			player = SpellCasting.Instances[0].transform;
		}
		else
		{
			SceneManager.sceneLoaded += SceneLoaded;
		}
	}

	private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.name == "Player")
		{
			player = SpellCasting.Instances[0].transform;
			SceneManager.sceneLoaded -= SceneLoaded;
		}
	}

	private void Start()
	{
		if (WaveManager.Instance != null)
		{
			WaveManager.Instance.OnWalkedRoom += Optimise;
		}
	}

	public void OptimiseAtEndOfFrame(Transform room)
	{
		StartCoroutine(OptimiseAtEndOfFrameCoroutine(room));
	}

	private IEnumerator OptimiseAtEndOfFrameCoroutine(Transform room)
	{
		yield return new WaitForEndOfFrame();
		Optimise(room);
	}

	public void Optimise(Transform room)
	{
		if (!(player == null))
		{
			Vector3 position = player.transform.position;
			if (room != null)
			{
				position = room.transform.position;
			}
			float num = 25f;
			if ((float)Screen.width / (float)Screen.height > 2f)
			{
				num = 40f;
			}
			if (Vector3.Distance(position, base.transform.position) > num)
			{
				base.gameObject.SetActive(value: false);
			}
			else
			{
				base.gameObject.SetActive(value: true);
			}
		}
	}
}
