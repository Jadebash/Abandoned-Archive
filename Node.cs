using System;
using UnityEngine;

public class Node : IHeapItem<Node>, IComparable<Node>
{
	public bool accessible;

	public Vector2 worldPosition;

	public Node parent;

	public int gridX;

	public int gridY;

	public int gCost;

	public int hCost;

	private int heapIndex;

	public int fCost => gCost + hCost;

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

	public Node(bool _accessible, Vector2 _worldPosition, int _gridX, int _gridY)
	{
		accessible = _accessible;
		worldPosition = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int CompareTo(Node other)
	{
		int num = fCost.CompareTo(other.fCost);
		if (num == 0)
		{
			num = hCost.CompareTo(other.hCost);
		}
		return -num;
	}
}
