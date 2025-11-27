using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	[Serializable]
	private class MouseCursorPosition
	{
		public PlayerInput playerInput;

		public Vector2 position;

		public bool isMouse;
	}

	public static CameraController Instance;

	public List<Transform> targets = new List<Transform>();

	public Transform lookTarget;

	public Transform biasTarget;

	private Rigidbody2D targetRB;

	public float deadzone;

	public float cursorInfluence;

	public float smoothing;

	public float controllerSmoothing = 10f;

	[Header("Co-op Camera")]
	public float coopMinSize = 4f;

	public float coopMaxSize = 6f;

	public float coopSizeDivisor = 1.25f;

	public float coopZoomSmoothing = 3f;

	private List<MouseCursorPosition> mouseCursorPositions = new List<MouseCursorPosition>();

	private bool isLockedDuringFloorTransition;

	private Vector3 lockedPosition;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		UnityEngine.Object.FindFirstObjectByType<PlayerInput>();
	}

	private void Update()
	{
		if (isLockedDuringFloorTransition)
		{
			base.transform.position = lockedPosition;
			return;
		}
		Vector2 zero = Vector2.zero;
		foreach (MouseCursorPosition mouseCursorPosition in mouseCursorPositions)
		{
			zero += mouseCursorPosition.position;
		}
		zero /= (float)mouseCursorPositions.Count;
		Vector3 vector = Camera.main.ScreenToViewportPoint(zero);
		vector.x = Mathf.Clamp(vector.x, 0f, 1f) - 0.5f;
		vector.y = Mathf.Clamp(vector.y, 0f, 1f) - 0.5f;
		if (Mathf.Abs(vector.x) < deadzone && Mathf.Abs(vector.y) < deadzone)
		{
			vector.x = 0f;
			vector.y = 0f;
		}
		Vector3 vector2 = Vector3.zero;
		if (targets.Count == 1)
		{
			if (targets[0] != null)
			{
				vector2 = targets[0].position;
			}
		}
		else if (targets.Count > 1 && targets[0] != null)
		{
			Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
			for (int i = 1; i < targets.Count; i++)
			{
				if (targets[i] != null)
				{
					bounds.Encapsulate(targets[i].position);
				}
			}
			vector2 = bounds.center;
			float value = bounds.size.magnitude / coopSizeDivisor;
			Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Mathf.Clamp(value, coopMinSize, coopMaxSize), Time.deltaTime * coopZoomSmoothing);
		}
		if (lookTarget != null)
		{
			vector2 = lookTarget.position;
		}
		if (biasTarget != null)
		{
			Vector3 vector3 = biasTarget.position - vector2;
			if (vector3.magnitude > 1.5f)
			{
				vector3 = vector3.normalized * 1.5f;
			}
			vector2 += vector3;
		}
		float num = 1f;
		if (mouseCursorPositions.Count > 1)
		{
			num = 0f;
		}
		float num2 = (mouseCursorPositions[0].isMouse ? smoothing : controllerSmoothing);
		base.transform.position = Vector3.Lerp(base.transform.position, vector2 + vector * cursorInfluence * num * SaveManager.Instance.currentSave.cursorCameraInfluence, Time.deltaTime * num2);
	}

	public void UpdateMouseCursorPosition(InputAction.CallbackContext context, PlayerInput playerInput)
	{
		if (!context.canceled)
		{
			MouseCursorPosition mouseCursorPosition = mouseCursorPositions.Find((MouseCursorPosition x) => x.playerInput == playerInput);
			if (mouseCursorPosition == null)
			{
				Debug.LogWarning("MouseCursorPosition not found for player " + playerInput.name);
			}
			else if (!(mouseCursorPosition.isMouse = context.control.device.ToString().Contains("Mouse")))
			{
				Vector2 vector = context.ReadValue<Vector2>();
				mouseCursorPosition.position.x = (vector.x + 1f) / 2f * (float)Screen.width;
				mouseCursorPosition.position.y = (vector.y + 1f) / 2f * (float)Screen.height;
			}
			else
			{
				mouseCursorPosition.position = context.ReadValue<Vector2>();
			}
		}
	}

	public void AddPlayer(PlayerInput input)
	{
		targets.Add(input.transform);
		mouseCursorPositions.Add(new MouseCursorPosition
		{
			playerInput = input,
			position = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f),
			isMouse = (input.currentControlScheme == "Keyboard&Mouse")
		});
		InputAction inputAction = input.actions.FindActionMap(input.defaultActionMap).FindAction("Look");
		if (inputAction != null)
		{
			inputAction.performed += delegate(InputAction.CallbackContext context)
			{
				UpdateMouseCursorPosition(context, input);
			};
			inputAction.canceled += delegate(InputAction.CallbackContext context)
			{
				UpdateMouseCursorPosition(context, input);
			};
		}
	}

	public float GetMaxCoopDistance()
	{
		return coopMaxSize * coopSizeDivisor;
	}

	public void LockAtPosition(Vector3 position)
	{
		isLockedDuringFloorTransition = true;
		lockedPosition = position;
	}

	public void Unlock()
	{
		isLockedDuringFloorTransition = false;
	}
}
