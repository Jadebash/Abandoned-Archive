using System.Collections;
using UnityEngine;

public class FadeOutAndDestroy : MonoBehaviour
{
	public float fadeTime = 1f;

	public float offsetTime;

	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
		{
			Debug.LogWarning("SpriteRenderer component not found on " + base.gameObject.name);
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				Debug.LogWarning("SpriteRenderer component not found on any child of " + base.gameObject.name);
			}
		}
		StartCoroutine(FadeOutAndDestroyCoroutine());
	}

	private IEnumerator FadeOutAndDestroyCoroutine()
	{
		yield return new WaitForSeconds(offsetTime);
		float startTime = Time.time;
		while (Time.time - startTime < fadeTime)
		{
			if (spriteRenderer != null)
			{
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f - (Time.time - startTime) / fadeTime);
			}
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield return null;
	}
}
