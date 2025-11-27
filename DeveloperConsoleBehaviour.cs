using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeveloperConsoleBehaviour : MonoBehaviour
{
	[SerializeField]
	private ConsoleCommand[] commands = new ConsoleCommand[0];

	[Header("UI")]
	[SerializeField]
	private GameObject uiCanvas;

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private TextMeshProUGUI log;

	[SerializeField]
	private ScrollRect scrollRect;

	[Header("Settings")]
	[SerializeField]
	private int maxHistorySize = 50;

	public static DeveloperConsoleBehaviour Instance;

	private DeveloperConsole developerConsole;

	private List<string> commandHistory = new List<string>();

	private int historyIndex = -1;

	private DeveloperConsole DeveloperConsole
	{
		get
		{
			if (developerConsole != null)
			{
				return developerConsole;
			}
			return developerConsole = new DeveloperConsole(commands);
		}
	}

	private IEnumerator MoveInputFieldCaret()
	{
		yield return new WaitForEndOfFrame();
		inputField.caretPosition = inputField.text.Length;
		inputField.ForceLabelUpdate();
	}

	private IEnumerator ScrollToBottom()
	{
		yield return new WaitForEndOfFrame();
		if (scrollRect != null)
		{
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		log.text = InitialMessage();
	}

	public void Toggle()
	{
		if (uiCanvas.activeSelf)
		{
			uiCanvas.SetActive(value: false);
			Manager.Instance?.UnpauseGame();
			return;
		}
		uiCanvas.SetActive(value: true);
		Manager.Instance?.PauseGame();
		inputField.ActivateInputField();
		StartCoroutine(ScrollToBottom());
	}

	public void Autocomplete()
	{
		if (uiCanvas.activeSelf)
		{
			string text = DeveloperConsole.Autocomplete(inputField.text);
			if (text != null)
			{
				inputField.text = text;
				StartCoroutine(MoveInputFieldCaret());
			}
		}
	}

	public void ProcessCommand(string inputValue)
	{
		if (inputValue == string.Empty)
		{
			return;
		}
		if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != inputValue)
		{
			commandHistory.Add(inputValue);
			if (commandHistory.Count > maxHistorySize)
			{
				commandHistory.RemoveAt(0);
			}
		}
		historyIndex = commandHistory.Count;
		CommandResponse commandResponse = DeveloperConsole.ProcessCommand(inputValue);
		Debug.Log("Executed Command: " + inputValue);
		TextMeshProUGUI textMeshProUGUI = log;
		textMeshProUGUI.text = textMeshProUGUI.text + "\n> " + inputValue;
		if (commandResponse == null)
		{
			log.text += "\n<color=red>Invalid Command. Type 'help' for a list of commands.</color>";
		}
		else if (commandResponse.success)
		{
			TextMeshProUGUI textMeshProUGUI2 = log;
			textMeshProUGUI2.text = textMeshProUGUI2.text + "\n" + commandResponse.response;
		}
		else
		{
			TextMeshProUGUI textMeshProUGUI3 = log;
			textMeshProUGUI3.text = textMeshProUGUI3.text + "\n<color=red>" + commandResponse.response + "</color>";
		}
		StartCoroutine(ScrollToBottom());
		inputField.text = string.Empty;
		inputField.ActivateInputField();
	}

	public void NavigateHistory(int direction)
	{
		if (uiCanvas.activeSelf && commandHistory.Count != 0)
		{
			historyIndex += direction;
			historyIndex = Mathf.Clamp(historyIndex, 0, commandHistory.Count);
			if (historyIndex < commandHistory.Count)
			{
				inputField.text = commandHistory[historyIndex];
				StartCoroutine(MoveInputFieldCaret());
			}
			else
			{
				inputField.text = string.Empty;
			}
		}
	}

	public void ClearLog()
	{
		log.text = "Console cleared.\n" + InitialMessage();
		StartCoroutine(ScrollToBottom());
	}

	public string HelpMessage()
	{
		string text = "\n<b>Available Commands:</b>\n";
		text += "────────────────────────────────────────\n";
		ConsoleCommand[] array = commands;
		foreach (ConsoleCommand consoleCommand in array)
		{
			text = text + "<color=#00DDFF><b>" + consoleCommand.CommandWord + "</b></color>";
			if (!string.IsNullOrEmpty(consoleCommand.Description))
			{
				string text2 = consoleCommand.Description.Split('\n')[0];
				text = text + "\n  <color=#AAAAAA>" + text2 + "</color>";
			}
			text += "\n\n";
		}
		text += "────────────────────────────────────────\n";
		return text + "<i>Type 'help <command>' for more details</i>";
	}

	public string InitialMessage()
	{
		return string.Concat("Abandoned Archive Developer Console\n" + "Type 'help' for a list of commands.\n", "Type 'help <command>' for detailed info on a specific command.\n");
	}

	public string GetCommandHelp(string commandName)
	{
		ConsoleCommand[] array = commands;
		foreach (ConsoleCommand consoleCommand in array)
		{
			if (consoleCommand.CommandWord.Equals(commandName, StringComparison.OrdinalIgnoreCase))
			{
				string text = "────────────────────────────────────────\n";
				text = text + "<color=#00DDFF><b>" + consoleCommand.CommandWord + "</b></color>\n";
				text += "────────────────────────────────────────\n";
				if (!string.IsNullOrEmpty(consoleCommand.Description))
				{
					text = text + "\n" + consoleCommand.Description + "\n";
				}
				if (!string.IsNullOrEmpty(consoleCommand.Usage))
				{
					text = text + "\n<color=#FFD700><b>Usage:</b></color>\n<color=#CCCCCC>" + consoleCommand.Usage + "</color>";
				}
				return text + "\n────────────────────────────────────────";
			}
		}
		return "<color=red>Command '" + commandName + "' not found.</color>";
	}
}
