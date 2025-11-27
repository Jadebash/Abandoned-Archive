using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScaleUI : MonoBehaviour
{
	private RectTransform rectTransform;

	public Vector3 maxScale = new Vector3(2f, 2f, 2f);

	public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		SetScale(UIScaling.GetUIScale());
		UIScaling.OnUIScaleChanged += OnUIScaleChanged;
	}

	private void SetScale(float scaleFactor)
	{
		Vector3 localScale = new Vector3(scaleFactor, scaleFactor, 1f);
		if (localScale.x > maxScale.x)
		{
			localScale.x = maxScale.x;
		}
		if (localScale.y > maxScale.y)
		{
			localScale.y = maxScale.y;
		}
		if (localScale.z > maxScale.z)
		{
			localScale.z = maxScale.z;
		}
		if (localScale.x < minScale.x)
		{
			localScale.x = minScale.x;
		}
		if (localScale.y < minScale.y)
		{
			localScale.y = minScale.y;
		}
		if (localScale.z < minScale.z)
		{
			localScale.z = minScale.z;
		}
		rectTransform.localScale = localScale;
	}

	private void OnUIScaleChanged(float scale)
	{
		SetScale(scale);
	}

	private void OnDestroy()
	{
		UIScaling.OnUIScaleChanged -= OnUIScaleChanged;
	}
}
