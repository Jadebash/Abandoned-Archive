using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
	public bool displayBoundsGizmo;

	public bool visualiseNodes;

	public LayerMask unaccessibleMask;

	public Vector2 offset;

	public Vector2 gridWorldSize;

	public float nodeRadius;

	[HideInInspector]
	public bool gridResolved;

	private Node[,] grid;

	private float nodeDiameter;

	private int gridSizeX;

	private int gridSizeY;

	public bool resolveGridOnStart;

	[HideInInspector]
	public int maxSize => gridSizeX * gridSizeY;

	private void OnDrawGizmos()
	{
		if (!displayBoundsGizmo)
		{
			return;
		}
		Gizmos.DrawWireCube(offset, new Vector3(gridWorldSize.x, gridWorldSize.y, 0f));
		if (!visualiseNodes || grid == null)
		{
			return;
		}
		for (int i = 0; i < gridSizeX; i++)
		{
			for (int j = 0; j < gridSizeY; j++)
			{
				if (!grid[i, j].accessible)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.white;
				}
				Gizmos.DrawSphere(grid[i, j].worldPosition, nodeRadius / 2f);
			}
		}
	}

	public Node NodeFromWorldPoint(Vector2 worldPosition)
	{
		worldPosition -= offset;
		float value = (worldPosition.x + gridWorldSize.x / 2f) / gridWorldSize.x;
		float value2 = (worldPosition.y + gridWorldSize.y / 2f) / gridWorldSize.y;
		value = Mathf.Clamp01(value);
		value2 = Mathf.Clamp01(value2);
		int num = Mathf.RoundToInt((float)(gridSizeX - 1) * value);
		int num2 = Mathf.RoundToInt((float)(gridSizeY - 1) * value2);
		return grid[num, num2];
	}

	public List<Node> GetNeighbours(Node node)
	{
		List<Node> list = new List<Node>();
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					int num = node.gridX + i;
					int num2 = node.gridY + j;
					if (num >= 0 && num < gridSizeX && num2 >= 0 && num2 < gridSizeY)
					{
						list.Add(grid[num, num2]);
					}
				}
			}
		}
		return list;
	}

	private void Start()
	{
		if (resolveGridOnStart)
		{
			ResolveGrid();
		}
	}

	public void ResolveGrid()
	{
		StartCoroutine(ResolveGridCoroutine());
	}

	private IEnumerator ResolveGridCoroutine()
	{
		yield return new WaitForEndOfFrame();
		nodeDiameter = nodeRadius * 2f;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		grid = new Node[gridSizeX, gridSizeY];
		Vector2 vector = offset - Vector2.right * gridWorldSize.x / 2f - Vector2.up * gridWorldSize.y / 2f;
		for (int i = 0; i < gridSizeX; i++)
		{
			for (int j = 0; j < gridSizeY; j++)
			{
				Vector2 vector2 = vector + Vector2.right * ((float)i * nodeDiameter + nodeRadius) + Vector2.up * ((float)j * nodeDiameter + nodeRadius);
				bool accessible = Physics2D.OverlapCircle(vector2, nodeRadius, unaccessibleMask.value) == null;
				grid[i, j] = new Node(accessible, vector2, i, j);
			}
		}
		gridResolved = true;
	}
}
