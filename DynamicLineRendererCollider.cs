using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class DynamicLineRendererCollider : MonoBehaviour
{
	private LineRenderer lr;

	private EdgeCollider2D col;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
		col = GetComponent<EdgeCollider2D>();
	}

	private void LateUpdate()
	{
		int positionCount = lr.positionCount;
		Vector2[] array = new Vector2[positionCount];
		for (int i = 0; i < positionCount; i++)
		{
			array[i] = lr.GetPosition(i);
		}
		col.points = array;
	}
}
