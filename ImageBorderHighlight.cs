using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBorderHighlight : MonoBehaviour
{
	[Header("Border Settings")]
	public Color borderColor = Color.yellow;

	public float maxExpansion = 30f;

	public float expansionTime = 1f;

	public float spawnInterval = 0.5f;

	public bool activateOnStart = true;

	[Header("Optional")]
	public Sprite borderSpriteOverride;

	public float destroyAfter;

	public bool destroyComponentOnly;

	private Image targetImage;

	private SpriteRenderer targetSpriteRenderer;

	private RectTransform targetRect;

	private List<GameObject> activeBorders = new List<GameObject>();

	private bool isUI => targetImage != null;

	public void SingleBurst()
	{
		StartCoroutine(SpawnOneBorder());
	}

	private void Awake()
	{
		targetImage = GetComponent<Image>();
		targetSpriteRenderer = GetComponent<SpriteRenderer>();
		if (targetImage != null)
		{
			targetRect = targetImage.rectTransform;
		}
	}

	private void OnEnable()
	{
		if (targetImage == null && targetSpriteRenderer == null)
		{
			Debug.LogWarning(base.name + " has no Image or SpriteRenderer. Disabling.");
			base.enabled = false;
		}
		else if (activateOnStart)
		{
			StartCoroutine(SpawnBorders());
			if (destroyAfter > 0f)
			{
				StartCoroutine(DestroyAfterTime());
			}
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		foreach (GameObject activeBorder in activeBorders)
		{
			if (activeBorder != null)
			{
				Object.Destroy(activeBorder);
			}
		}
		activeBorders.Clear();
	}

	private IEnumerator SpawnOneBorder()
	{
		StartCoroutine(AnimateBorder());
		yield return new WaitForSeconds(spawnInterval);
	}

	private IEnumerator SpawnBorders()
	{
		while (true)
		{
			StartCoroutine(AnimateBorder());
			yield return new WaitForSeconds(spawnInterval);
		}
	}

	private IEnumerator AnimateBorder()
	{
		GameObject borderGO = new GameObject("BorderHighlight");
		activeBorders.Add(borderGO);
		if (isUI)
		{
			borderGO.transform.SetParent(base.transform.parent, worldPositionStays: false);
			borderGO.transform.SetAsFirstSibling();
			Image borderImage = borderGO.AddComponent<Image>();
			borderImage.sprite = (borderSpriteOverride ? borderSpriteOverride : targetImage.sprite);
			borderImage.raycastTarget = false;
			borderImage.color = borderColor;
			borderImage.type = targetImage.type;
			RectTransform borderRect = borderGO.GetComponent<RectTransform>();
			borderRect.anchorMin = targetRect.anchorMin;
			borderRect.anchorMax = targetRect.anchorMax;
			borderRect.pivot = targetRect.pivot;
			borderRect.position = targetRect.position;
			borderRect.sizeDelta = targetRect.sizeDelta;
			float elapsed = 0f;
			Color startColor = borderColor;
			while (elapsed < expansionTime)
			{
				elapsed += Time.deltaTime;
				float num = elapsed / expansionTime;
				float num2 = 1f + maxExpansion / targetRect.sizeDelta.x * num;
				borderRect.localScale = new Vector3(num2, num2, 1f);
				Color color = startColor;
				color.a = Mathf.Lerp(startColor.a, 0f, num);
				borderImage.color = color;
				yield return null;
			}
		}
		else
		{
			borderGO.transform.SetParent(base.transform.parent);
			borderGO.transform.position = base.transform.position;
			borderGO.transform.localScale = base.transform.localScale;
			SpriteRenderer borderRenderer = borderGO.AddComponent<SpriteRenderer>();
			borderRenderer.sprite = (borderSpriteOverride ? borderSpriteOverride : targetSpriteRenderer.sprite);
			borderRenderer.color = borderColor;
			borderRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;
			borderRenderer.sortingOrder = targetSpriteRenderer.sortingOrder - 1;
			borderRenderer.flipX = targetSpriteRenderer.flipX;
			float elapsed = 0f;
			Color startColor = borderColor;
			Vector3 baseScale = base.transform.localScale;
			while (elapsed < expansionTime)
			{
				elapsed += Time.deltaTime;
				float num3 = elapsed / expansionTime;
				float num4 = 1f + maxExpansion / 100f * num3;
				borderGO.transform.localScale = baseScale * num4;
				Color color2 = startColor;
				color2.a = Mathf.Lerp(startColor.a, 0f, num3);
				borderRenderer.color = color2;
				yield return null;
			}
		}
		activeBorders.Remove(borderGO);
		Object.Destroy(borderGO);
	}

	private IEnumerator DestroyAfterTime()
	{
		yield return new WaitForSeconds(destroyAfter);
		if (destroyComponentOnly)
		{
			Object.Destroy(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
