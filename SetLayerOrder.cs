using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SetLayerOrder : MonoBehaviour
{
	private SpriteRenderer sr;

	public float yLevelOffset;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		sr.sortingOrder = Mathf.RoundToInt((0f - (base.transform.position.y + yLevelOffset)) * 100f);
	}
}
