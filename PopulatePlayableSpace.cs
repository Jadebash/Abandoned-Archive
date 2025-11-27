using UnityEngine;
using UnityEngine.SceneManagement;

public class PopulatePlayableSpace : MonoBehaviour
{
	private ParticleSystem particles;

	private void Start()
	{
		particles = GetComponent<ParticleSystem>();
		HookToGenerator();
		SceneManager.sceneLoaded += LoadedScene;
		AddPlayableSpaceColliders();
	}

	private void LoadedScene(Scene scene, LoadSceneMode sceneLoadMode)
	{
		HookToGenerator();
	}

	private void HookToGenerator()
	{
		if (MapGenerator.Instance != null)
		{
			MapGenerator.Instance.OnGenerationComplete += AddPlayableSpaceColliders;
		}
	}

	private void AddPlayableSpaceColliders()
	{
		ParticleSystem.TriggerModule trigger = particles.trigger;
		for (int i = 0; i < trigger.colliderCount; i++)
		{
			trigger.RemoveCollider(i);
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("PlayableSpace");
		if (array.Length == 0)
		{
			return;
		}
		trigger.enabled = true;
		for (int j = 0; j < array.Length; j++)
		{
			if (!(array[j] == null))
			{
				trigger.AddCollider(array[j].GetComponent<Collider2D>());
			}
		}
	}

	private void OnDestroy()
	{
		if (MapGenerator.Instance != null)
		{
			MapGenerator.Instance.OnGenerationComplete += AddPlayableSpaceColliders;
		}
		SceneManager.sceneLoaded -= LoadedScene;
	}
}
