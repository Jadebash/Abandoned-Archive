using UnityEngine;
using UnityEngine.InputSystem;

public class DeveloperConsolePlayer : MonoBehaviour
{
	public void Toggle(InputAction.CallbackContext context)
	{
		if (context.started && 0 == 0 && SaveManager.Instance.currentSave.developer)
		{
			DeveloperConsoleBehaviour.Instance?.Toggle();
		}
	}

	public void Autocomplete(InputAction.CallbackContext context)
	{
		_ = context.started;
	}

	public void NavigateUp(InputAction.CallbackContext context)
	{
		_ = context.started;
	}

	public void NavigateDown(InputAction.CallbackContext context)
	{
		_ = context.started;
	}
}
