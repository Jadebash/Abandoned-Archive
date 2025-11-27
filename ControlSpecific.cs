using System.Collections.Generic;
using UnityEngine;

public class ControlSpecific : MonoBehaviour
{
	public ControlType controlType;

	public List<GameObject> controlGameObjects;

	private void Start()
	{
		UpdateControl();
	}

	public void UpdateControl()
	{
		bool allActive = false;
		bool allActive2 = false;
		foreach (ControllerAim instance in ControllerAim.Instances)
		{
			if (instance.usingController)
			{
				allActive = true;
			}
			else
			{
				allActive2 = true;
			}
		}
		switch (controlType)
		{
		case ControlType.Controller:
			SetAllActive(allActive);
			break;
		case ControlType.Keyboard:
			SetAllActive(allActive2);
			break;
		}
	}

	public void SetAllActive(bool active)
	{
		foreach (GameObject controlGameObject in controlGameObjects)
		{
			controlGameObject.SetActive(active);
		}
	}
}
