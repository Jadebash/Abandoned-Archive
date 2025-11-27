using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ControllerAim : MonoBehaviour
{
	public static List<ControllerAim> Instances = new List<ControllerAim>();

	public RectTransform controllerCrosshair;

	public bool usingController;

	[HideInInspector]
	public bool hideCrosshair;

	[HideInInspector]
	public bool isInDeadzone = true;

	private ControlSpecific[] controlSpecificObjects;

	private PlayerInput playerInput;

	private void Awake()
	{
		Instances.Add(this);
		playerInput = base.transform.parent.GetComponent<PlayerInput>();
	}

	private void OnDestroy()
	{
		Instances.Remove(this);
	}

	private void OnEnable()
	{
		playerInput.onControlsChanged += OnControlsChanged;
	}

	private void OnDisable()
	{
		playerInput.onControlsChanged -= OnControlsChanged;
	}

	public void Start()
	{
		controlSpecificObjects = Object.FindObjectsOfType<ControlSpecific>();
		OnControlsChanged(playerInput);
		SceneManager.sceneLoaded += delegate
		{
			controlSpecificObjects = Object.FindObjectsOfType<ControlSpecific>();
			OnControlsChanged(playerInput);
		};
	}

	public void Look(InputAction.CallbackContext context)
	{
		if (!usingController || !(controllerCrosshair != null))
		{
			return;
		}
		Vector2 vector = context.ReadValue<Vector2>();
		vector.x = (vector.x + 1f) / 2f;
		vector.y = (vector.y + 1f) / 2f;
		if ((double)Mathf.Abs(vector[0]) != 0.5 || (double)Mathf.Abs(vector[1]) != 0.5)
		{
			isInDeadzone = false;
			if (!hideCrosshair)
			{
				controllerCrosshair.gameObject.SetActive(value: true);
			}
			controllerCrosshair.anchoredPosition = new Vector3(vector.x * 1920f, vector.y * 1080f, 0f);
		}
		else
		{
			isInDeadzone = true;
			controllerCrosshair.gameObject.SetActive(value: false);
		}
	}

	public void OnControlsChanged(PlayerInput input)
	{
		if (controlSpecificObjects == null)
		{
			return;
		}
		if (input.currentControlScheme == "Gamepad")
		{
			usingController = true;
		}
		else
		{
			usingController = false;
		}
		bool flag = false;
		foreach (ControllerAim instance in Instances)
		{
			if (!instance.usingController)
			{
				flag = true;
			}
		}
		UIControl.UpdateAllTooltips();
		ControlSpecific[] array = controlSpecificObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateControl();
		}
		if (!flag)
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
