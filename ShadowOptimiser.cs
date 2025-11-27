using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowOptimiser : MonoBehaviour
{
	private Transform player;

	private Light2D light2D;

	private float shadowIntensity;

	private void Awake()
	{
		light2D = GetComponent<Light2D>();
		shadowIntensity = light2D.shadowIntensity;
		light2D.shadowIntensity = 0f;
	}

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player")?.transform;
		if (player != null)
		{
			UpdateLight();
			InvokeRepeating("UpdateLight", Random.Range(0.1f, 1f), 1f);
		}
	}

	private void OnEnable()
	{
		if (player != null)
		{
			UpdateLight();
		}
	}

	public void UpdateLight()
	{
		if ((!(MapGenerator.Instance != null) || !MapGenerator.Instance.dontOptimise) && base.gameObject.activeInHierarchy && !(player == null) && base.gameObject.activeSelf)
		{
			if (Vector3.Distance(player.position, base.transform.position) < light2D.pointLightOuterRadius + 8f)
			{
				light2D.shadowIntensity = shadowIntensity;
			}
			else
			{
				light2D.shadowIntensity = 0f;
			}
		}
	}
}
