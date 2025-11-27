using UnityEngine;
using UnityEngine.EventSystems;

public class DebugEventSystem : MonoBehaviour
{
	private void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			Debug.Log(EventSystem.current.currentSelectedGameObject);
		}
	}
}
