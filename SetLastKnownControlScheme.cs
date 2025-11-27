using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class SetLastKnownControlScheme : MonoBehaviour
{
	private PlayerInput playerInput;

	private string cache = "";

	private void OnEnable()
	{
		playerInput = GetComponent<PlayerInput>();
		if (!playerInput.neverAutoSwitchControlSchemes && !Coop())
		{
			playerInput.onControlsChanged += OnControlsChanged;
			if (Manager.Instance != null)
			{
				playerInput.SwitchCurrentControlScheme(Manager.Instance.LastKnownControlScheme);
			}
			OnControlsChanged(playerInput);
		}
	}

	private bool Coop()
	{
		if (PlayerInputManager.instance != null)
		{
			return PlayerInputManager.instance.joiningEnabled;
		}
		return false;
	}

	private void OnControlsChanged(PlayerInput input)
	{
		if (!input.neverAutoSwitchControlSchemes && !Coop() && Manager.Instance != null)
		{
			Manager.Instance.LastKnownControlScheme = input.currentControlScheme;
		}
	}

	private void OnDisable()
	{
		playerInput.onControlsChanged -= OnControlsChanged;
	}
}
