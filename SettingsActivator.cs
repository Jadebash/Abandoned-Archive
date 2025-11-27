using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsActivator : MonoBehaviour
{
	public void OpenSettings(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Settings.Instance.PressMenu(sound: true, base.gameObject);
		}
	}
}
