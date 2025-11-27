using UnityEngine;
using UnityEngine.InputSystem;

public class LoadSavedBindings : MonoBehaviour
{
	private void Start()
	{
		PlayerInput component = GetComponent<PlayerInput>();
		if (SaveManager.Instance != null)
		{
			component.actions.LoadBindingOverridesFromJson(SaveManager.Instance.currentSave.keybindOverrides);
			UIControl.UpdateAllTooltips();
		}
	}
}
