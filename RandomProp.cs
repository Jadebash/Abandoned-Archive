using UnityEngine;

public class RandomProp : MonoBehaviour
{
	public float chance = 50f;

	private void Awake()
	{
		if (Random.Range(0f, 100f) < 100f - chance)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
