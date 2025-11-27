using System.Collections.Generic;
using UnityEngine;

public class EnableRandomChild : MonoBehaviour
{
	public bool chanceToEnableNone = true;

	private void Start()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
			list.Add(item);
		}
		if (!chanceToEnableNone || !(Random.Range(0f, 100f) < 50f))
		{
			list[Random.Range(0, list.Count)].gameObject.SetActive(value: true);
		}
	}
}
