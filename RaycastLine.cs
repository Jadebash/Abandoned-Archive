using UnityEngine;

public class RaycastLine : MonoBehaviour
{
	private LineRenderer line;

	private BoxCollider2D col;

	public float maxLength = 100f;

	public Transform freeformLight;

	public LayerMask raycastLayerMask;

	public float freeformLightScaleDivider = 2.5f;

	private void Start()
	{
		line = GetComponent<LineRenderer>();
		col = GetComponent<BoxCollider2D>();
		UpdateLine();
	}

	public void OnEnable()
	{
		if (line != null)
		{
			UpdateLine();
		}
	}

	private void Update()
	{
		UpdateLine();
	}

	public void UpdateLine()
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, base.transform.right, maxLength, raycastLayerMask.value);
		line.SetPosition(0, base.transform.position);
		if (raycastHit2D.collider != null)
		{
			line.SetPosition(1, raycastHit2D.point);
			float num = Vector3.Distance(raycastHit2D.point, base.transform.position);
			if (col != null)
			{
				col.size = new Vector2(num / 2f, col.size.y);
				col.offset = new Vector2(num / 4f, col.offset.y);
			}
			if (freeformLight != null)
			{
				freeformLight.localScale = new Vector3(num / freeformLightScaleDivider, 1f, 1f);
			}
		}
		else
		{
			line.SetPosition(1, base.transform.position + base.transform.right * maxLength);
			if (col != null)
			{
				col.size = new Vector2(maxLength, col.size.y);
				col.offset = new Vector2(maxLength / 2f, col.offset.y);
			}
			if (freeformLight != null)
			{
				freeformLight.localScale = new Vector3(maxLength / freeformLightScaleDivider, 1f, 1f);
			}
		}
	}
}
