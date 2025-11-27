using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomRelic : MonoBehaviour
{
	public Relic[] negativeRelics;

	public GameObject leverLeft;

	private EventRoomLever leverLeftComponent;

	private Relic selectedRelic;

	private void Start()
	{
		if (Random.Range(0, 2) == 0 && negativeRelics != null && negativeRelics.Length != 0)
		{
			List<Relic> playerRelics = RelicCollector.Instance.Relics();
			List<Relic> list = negativeRelics.Where((Relic relic) => relic != null && !playerRelics.Contains(relic)).ToList();
			if (list.Count > 0)
			{
				selectedRelic = list[Random.Range(0, list.Count)];
			}
			else
			{
				selectedRelic = RunManager.GetRelic();
			}
		}
		else
		{
			selectedRelic = RunManager.GetRelic();
		}
		if (leverLeft != null)
		{
			leverLeftComponent = leverLeft.GetComponent<EventRoomLever>();
			if (leverLeftComponent != null)
			{
				leverLeftComponent.OnLeave += HandleLeave;
			}
		}
	}

	private void HandleLeave()
	{
		Debug.Log("Leave event was triggered!");
		if (leverLeftComponent != null && leverLeftComponent.doesLeave)
		{
			_ = leverLeftComponent;
			if (selectedRelic != null)
			{
				RelicCollector.Instance.GetRelic(selectedRelic);
			}
		}
	}

	private void OnDestroy()
	{
		if (leverLeftComponent != null)
		{
			leverLeftComponent.OnLeave -= HandleLeave;
		}
	}

	private void Update()
	{
	}
}
