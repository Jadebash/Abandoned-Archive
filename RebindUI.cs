using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindUI : MonoBehaviour
{
	private PlayerInput playerInput;

	public InputActionReference action;

	[Tooltip("Keyboard&Mouse or Gamepad")]
	public string controlGroup = "Keyboard&Mouse";

	public InputBinding.DisplayStringOptions displayStringOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions;

	public TextMeshProUGUI controlText;

	public GameObject rebindOverlay;

	public TextMeshProUGUI rebindText;

	public LocalisedString bindingString;

	public LocalisedString waitingForInputString;

	private InputActionRebindingExtensions.RebindingOperation rebindOperation;

	private void OnEnable()
	{
		if (PlayerManager.Instance != null)
		{
			playerInput = PlayerManager.Instance.lastActivePlayerInput;
		}
		UpdateControlText();
	}

	public void StartRebinding()
	{
		int bindingIndex = GetBindingIndex();
		if (action.action.bindings[bindingIndex].isPartOfComposite)
		{
			PerformInteractiveRebind(action.action, bindingIndex, allCompositeParts: true);
		}
		else
		{
			PerformInteractiveRebind(action.action, bindingIndex);
		}
	}

	private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
	{
		if (playerInput != null)
		{
			playerInput.DeactivateInput();
		}
		rebindOperation?.Cancel();
		InputAction targetAction = action;
		if (playerInput != null)
		{
			targetAction = playerInput.actions[action.name];
		}
		bool wasEnabled = targetAction.enabled;
		if (wasEnabled)
		{
			targetAction.Disable();
		}
		rebindOperation = targetAction.PerformInteractiveRebinding(bindingIndex).OnCancel(delegate
		{
			rebindOverlay.SetActive(value: false);
			UpdateControlText();
			UIControl.UpdateAllTooltips();
			CleanUp();
		}).OnComplete(delegate
		{
			rebindOverlay.SetActive(value: false);
			UpdateControlText();
			UIControl.UpdateAllTooltips();
			CleanUp();
			if (SaveManager.Instance != null)
			{
				string text3 = null;
				if (playerInput != null && PlayerManager.players.Length != 0 && playerInput.gameObject == PlayerManager.players[0])
				{
					text3 = playerInput.actions.SaveBindingOverridesAsJson();
				}
				else if (playerInput == null)
				{
					text3 = targetAction.actionMap.asset.SaveBindingOverridesAsJson();
				}
				if (text3 != null)
				{
					Save currentSave = SaveManager.Instance.currentSave;
					currentSave.keybindOverrides = text3;
					SaveManager.Instance.currentSave = currentSave;
				}
			}
			if (allCompositeParts)
			{
				int num = bindingIndex + 1;
				if (num < action.bindings.Count && action.bindings[num].isPartOfComposite)
				{
					PerformInteractiveRebind(action, num, allCompositeParts: true);
				}
			}
		});
		string text = null;
		if (action.bindings[bindingIndex].isPartOfComposite)
		{
			text = bindingString.value + " '" + action.bindings[bindingIndex].name + "'. ";
		}
		rebindOverlay?.SetActive(value: true);
		if (rebindText != null)
		{
			string text2 = ((!string.IsNullOrEmpty(rebindOperation.expectedControlType)) ? (text + waitingForInputString.value + " [" + rebindOperation.expectedControlType + "]") : (text + waitingForInputString.value));
			rebindText.text = text2;
		}
		rebindOperation.Start();
		void CleanUp()
		{
			rebindOperation?.Dispose();
			rebindOperation = null;
			if (wasEnabled && !targetAction.enabled)
			{
				targetAction.Enable();
			}
			if (playerInput != null)
			{
				playerInput.ActivateInput();
			}
		}
	}

	private int GetBindingIndex()
	{
		int num = 0;
		foreach (InputBinding binding in action.action.bindings)
		{
			if (binding.groups.Contains(controlGroup))
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	private void UpdateControlText()
	{
		InputAction inputAction = action.action;
		if (playerInput != null)
		{
			inputAction = playerInput.actions[inputAction.name];
		}
		if (!(controlText != null) || !(action != null))
		{
			return;
		}
		string text = string.Empty;
		foreach (InputBinding binding in inputAction.bindings)
		{
			if (binding.groups.Contains(controlGroup))
			{
				text += binding.ToDisplayString(displayStringOptions);
				text += "/";
			}
		}
		text = text.TrimEnd('/');
		controlText.text = text;
	}
}
