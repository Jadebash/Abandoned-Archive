using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
	public static AnalyticsManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	private async void Start()
	{
		if (FloorManager.Instance != null)
		{
			FloorManager.Instance.OnStartFloor += OnStartFloor;
			FloorManager.Instance.OnCompleteFloor += OnCompleteFloor;
		}
		try
		{
			await UnityServices.InitializeAsync();
			AnalyticsService.Instance.StartDataCollection();
		}
		catch (Exception arg)
		{
			Debug.LogError($"Failed to initialize Unity Services: {arg}");
		}
	}

	private void OnStartFloor(int floorNumber)
	{
		LevelStartEvent e = new LevelStartEvent
		{
			FloorNum = floorNumber
		};
		AnalyticsService.Instance.RecordEvent(e);
	}

	private void OnCompleteFloor(int floorNumber)
	{
		LevelCompleteEvent e = new LevelCompleteEvent
		{
			FloorNum = floorNumber
		};
		AnalyticsService.Instance.RecordEvent(e);
	}

	public void CustomEvent(string eventName)
	{
		AnalyticsService.Instance.RecordEvent(eventName);
	}
}
