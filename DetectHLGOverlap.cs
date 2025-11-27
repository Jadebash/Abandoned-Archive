using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectHLGOverlap : MonoBehaviour
{
	public float yOffset;

	public float overlapThreshold = 2f;

	[SerializeField]
	private HorizontalLayoutGroup horizontalLayoutGroup;

	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void OnEnable()
	{
		if (horizontalLayoutGroup == null || this.rectTransform == null)
		{
			return;
		}
		List<RectTransform> list = new List<RectTransform>();
		for (int i = 0; i < horizontalLayoutGroup.transform.childCount; i++)
		{
			RectTransform rectTransform = horizontalLayoutGroup.transform.GetChild(i) as RectTransform;
			if (rectTransform != null)
			{
				list.Add(rectTransform);
			}
		}
		int num = 0;
		Rect worldRect = GetWorldRect(this.rectTransform);
		foreach (RectTransform item in list)
		{
			Rect worldRect2 = GetWorldRect(item);
			if (worldRect.Overlaps(worldRect2))
			{
				num++;
			}
		}
		if (!((float)num >= overlapThreshold))
		{
			return;
		}
		RectTransform component = horizontalLayoutGroup.GetComponent<RectTransform>();
		if (!(component == null))
		{
			Transform parent = this.rectTransform.parent;
			if (parent == null)
			{
				parent = this.rectTransform;
			}
			Vector3 position = component.TransformPoint(new Vector3(0f, yOffset, 0f));
			Vector3 vector = parent.InverseTransformPoint(position);
			Vector3 localPosition = this.rectTransform.localPosition;
			localPosition.y = vector.y;
			this.rectTransform.localPosition = localPosition;
		}
	}

	private Rect GetWorldRect(RectTransform rectTransform)
	{
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		float num = Mathf.Min(array[0].x, array[1].x, array[2].x, array[3].x);
		float num2 = Mathf.Max(array[0].x, array[1].x, array[2].x, array[3].x);
		float num3 = Mathf.Min(array[0].y, array[1].y, array[2].y, array[3].y);
		float num4 = Mathf.Max(array[0].y, array[1].y, array[2].y, array[3].y);
		return new Rect(num, num3, num2 - num, num4 - num3);
	}
}
