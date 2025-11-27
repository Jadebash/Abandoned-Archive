using UnityEngine;
using UnityEngine.InputSystem;

public class HideCursorForController : MonoBehaviour
{
	private PlayerInput playerInput;

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
	}

	private void OnEnable()
	{
		playerInput.onControlsChanged += OnControlsChanged;
	}

	private void OnDisable()
	{
		playerInput.onControlsChanged -= OnControlsChanged;
	}

	private void OnControlsChanged(PlayerInput input)
	{
		if (input.currentControlScheme == "Gamepad")
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
