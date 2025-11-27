using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowCasterOptimiser : MonoBehaviour
{
	public bool useParent = true;

	private Transform player;

	private ShadowCaster2D shadowCaster2D;

	private float shadowIntensity;

	private void Awake()
	{
		shadowCaster2D = GetComponent<ShadowCaster2D>();
	}

	private void Start()
	{
		if (SpellCasting.Instances.Count != 0)
		{
			player = SpellCasting.Instances[0].transform;
		}
		if (player != null)
		{
			UpdateCaster();
			InvokeRepeating("UpdateCaster", Random.Range(0.1f, 1f), 1f);
		}
	}

	private void OnEnable()
	{
		if (player != null)
		{
			UpdateCaster();
		}
	}

	public void UpdateCaster()
	{
		if ((!(MapGenerator.Instance != null) || !MapGenerator.Instance.dontOptimise) && base.gameObject.activeInHierarchy && !(player == null))
		{
			Vector3 position = base.transform.position;
			if (useParent)
			{
				position = base.transform.parent.position;
			}
			if (Vector3.Distance(player.position, position) < 14f)
			{
				shadowCaster2D.enabled = true;
			}
			else
			{
				shadowCaster2D.enabled = false;
			}
		}
	}
}
