using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
	private struct PathRequest
	{
		public Vector2 pathStart;

		public Vector2 pathEnd;

		public Action<Vector2[], bool> callback;

		public int wallProximityPenalty;

		public PathRequest(Vector2 _start, Vector2 _end, Action<Vector2[], bool> _callback, int _wallProximityPenalty)
		{
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			wallProximityPenalty = _wallProximityPenalty;
		}
	}

	public static PathRequestManager Instance;

	private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();

	private PathRequest currentPathRequest;

	private Pathfinding pathfinding;

	private bool isProcessingPath;

	private void Awake()
	{
		Instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}

	private void OnEnable()
	{
		Instance = this;
	}

	public static void RequestPath(Vector2 pathStart, Vector2 pathEnd, Action<Vector2[], bool> callback, int wallProximityPenalty = 0)
	{
		PathRequest item = new PathRequest(pathStart, pathEnd, callback, wallProximityPenalty);
		if (Instance != null)
		{
			Instance.pathRequestQueue.Enqueue(item);
			Instance.TryProcessNext();
		}
		else
		{
			callback(null, arg2: false);
		}
	}

	private void TryProcessNext()
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0)
		{
			currentPathRequest = pathRequestQueue.Dequeue();
			if (!base.gameObject.activeInHierarchy)
			{
				Debug.LogWarning("Pathfinding disabled.");
				currentPathRequest.callback(null, arg2: false);
				TryProcessNext();
			}
			else
			{
				isProcessingPath = true;
				pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.wallProximityPenalty);
			}
		}
	}

	public void FinishedProcessingPath(Vector2[] path, bool success)
	{
		currentPathRequest.callback(path, success);
		isProcessingPath = false;
		TryProcessNext();
	}
}
