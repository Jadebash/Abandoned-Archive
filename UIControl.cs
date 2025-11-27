using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControl : MonoBehaviour
{
	public string controlName = "roll";

	public bool getSubBinding;

	public int subBindingIndex;

	public PlayerInput playerInput;

	private TextMeshProUGUI text;

	public string customText = "";

	public InputBinding.DisplayStringOptions displayStringOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions;

	public bool ignoreCompositeBindings;

	public string[] ignoreBindingNames = new string[0];

	public string[] ignoreControlGroups = new string[0];

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
		if (playerInput == null)
		{
			playerInput = Object.FindObjectOfType<PlayerInput>();
		}
		if (playerInput != null)
		{
			UpdateTooltip(playerInput);
			playerInput.onControlsChanged += UpdateTooltip;
		}
	}

	private void Start()
	{
		FindPlayerInput();
	}

	private void OnEnable()
	{
		FindPlayerInput();
	}

	public void FindPlayerInput()
	{
		if (playerInput == null)
		{
			playerInput = Object.FindObjectOfType<PlayerInput>();
			if (playerInput != null)
			{
				UpdateTooltip(playerInput);
			}
		}
		else
		{
			UpdateTooltip(playerInput);
		}
	}

	public static void UpdateAllTooltips()
	{
		UIControl[] array = Object.FindObjectsOfType<UIControl>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateTooltip(null);
		}
	}

	public void UpdateTooltip(PlayerInput input)
	{
		if (input == null)
		{
			if (playerInput == null)
			{
				playerInput = Object.FindObjectOfType<PlayerInput>();
				if (playerInput == null)
				{
					Debug.LogWarning("UIControl: No PlayerInput found");
					return;
				}
			}
			input = playerInput;
		}
		if (this.text == null)
		{
			this.text = GetComponent<TextMeshProUGUI>();
		}
		InputAction inputAction = input.actions[controlName];
		string empty = string.Empty;
		string currentControlScheme = input.currentControlScheme;
		if (ignoreControlGroups != null && ignoreControlGroups.Length != 0)
		{
			string[] array = ignoreControlGroups;
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text) && currentControlScheme == text)
				{
					empty = "";
					if (customText == string.Empty)
					{
						this.text.text = empty;
					}
					else
					{
						this.text.text = customText.Replace("[control]", empty);
					}
					return;
				}
			}
		}
		List<string> list = new List<string>();
		foreach (InputBinding binding in inputAction.bindings)
		{
			if (!binding.groups.Contains(currentControlScheme) || (ignoreCompositeBindings && binding.isPartOfComposite))
			{
				continue;
			}
			bool flag = false;
			if (ignoreBindingNames != null && ignoreBindingNames.Length != 0)
			{
				string[] array = ignoreBindingNames;
				foreach (string text2 in array)
				{
					if (!string.IsNullOrEmpty(text2) && binding.name == text2)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				list.Add(binding.ToDisplayString(displayStringOptions));
			}
		}
		empty = ((list.Count == 0) ? "" : ((!getSubBinding) ? string.Join("/", list) : ((subBindingIndex >= list.Count) ? "" : list[subBindingIndex])));
		if (customText == string.Empty)
		{
			this.text.text = empty;
		}
		else
		{
			this.text.text = customText.Replace("[control]", empty);
		}
	}
}
