using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
	private NodeGrid nodeGrid;

	private PathRequestManager requestManager;

	private void Awake()
	{
		nodeGrid = GetComponent<NodeGrid>();
		requestManager = GetComponent<PathRequestManager>();
	}

	public void StartFindPath(Vector2 start, Vector2 target, int wallProximityPenalty = 0)
	{
		StartCoroutine(FindPath(start, target, wallProximityPenalty));
	}

	private Node FindClosestNode(Vector2 position)
	{
		Node node = nodeGrid.NodeFromWorldPoint(position);
		if (!node.accessible)
		{
			float num = float.PositiveInfinity;
			Node node2 = null;
			foreach (Node neighbour in nodeGrid.GetNeighbours(node))
			{
				if (neighbour.accessible)
				{
					float num2 = Vector2.Distance(position, neighbour.worldPosition);
					if (num2 < num)
					{
						num = num2;
						node2 = neighbour;
					}
				}
			}
			if (node2 != null)
			{
				node = node2;
			}
			else
			{
				float num3 = float.PositiveInfinity;
				Node node3 = null;
				foreach (Node neighbour2 in nodeGrid.GetNeighbours(node))
				{
					foreach (Node neighbour3 in nodeGrid.GetNeighbours(neighbour2))
					{
						if (neighbour3.accessible)
						{
							float num4 = Vector2.Distance(position, neighbour3.worldPosition);
							if (num4 < num3)
							{
								num3 = num4;
								node3 = neighbour3;
							}
						}
					}
				}
				if (node3 != null)
				{
					node = node3;
				}
			}
		}
		return node;
	}

	private IEnumerator FindPath(Vector2 start, Vector2 target, int wallProximityPenalty)
	{
		Vector2[] path = new Vector2[0];
		bool pathSuccess = false;
		if (!nodeGrid.gridResolved)
		{
			requestManager.FinishedProcessingPath(path, pathSuccess);
			yield break;
		}
		Node startNode = FindClosestNode(start);
		Node targetNode = FindClosestNode(target);
		if (startNode.accessible && targetNode.accessible)
		{
			Heap<Node> openSet = new Heap<Node>(nodeGrid.maxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			int iterations = 0;
			while (openSet.Count > 0)
			{
				iterations++;
				if (iterations > 500)
				{
					iterations = 0;
					yield return null;
				}
				Node node = openSet.RemoveFirst();
				closedSet.Add(node);
				if (node == targetNode)
				{
					pathSuccess = true;
					break;
				}
				foreach (Node neighbour in nodeGrid.GetNeighbours(node))
				{
					if (!neighbour.accessible || closedSet.Contains(neighbour))
					{
						continue;
					}
					int num = node.gCost + GetDistance(node, neighbour);
					if (IsAdjacentToWall(neighbour))
					{
						num += wallProximityPenalty;
					}
					if (num < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = num;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = node;
						if (!openSet.Contains(neighbour))
						{
							openSet.Add(neighbour);
						}
						else
						{
							openSet.UpdateItem(neighbour);
						}
					}
				}
			}
		}
		yield return null;
		if (pathSuccess)
		{
			path = RetracePath(startNode, targetNode);
		}
		requestManager.FinishedProcessingPath(path, pathSuccess);
	}

	private Vector2[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> list = new List<Node>();
		for (Node node = endNode; node != startNode; node = node.parent)
		{
			list.Add(node);
		}
		Vector2[] array = SimplifyPath(list);
		Array.Reverse(array);
		return array;
	}

	private Vector2[] SimplifyPath(List<Node> path)
	{
		List<Vector2> list = new List<Vector2>();
		_ = Vector2.zero;
		for (int i = 1; i < path.Count; i++)
		{
			list.Add(path[i].worldPosition);
		}
		return list.ToArray();
	}

	private int GetDistance(Node a, Node b)
	{
		int num = Mathf.Abs(a.gridX - b.gridX);
		int num2 = Mathf.Abs(a.gridY - b.gridY);
		if (num > num2)
		{
			return 14 * num2 + 10 * (num - num2);
		}
		return 14 * num + 10 * (num2 - num);
	}

	private bool IsAdjacentToWall(Node node)
	{
		foreach (Node neighbour in nodeGrid.GetNeighbours(node))
		{
			if (!neighbour.accessible)
			{
				return true;
			}
		}
		return false;
	}
}
