using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeveloperConsole
{
	public static bool hasBeenUsedThisSession;

	private readonly IEnumerable<ConsoleCommand> commands;

	public DeveloperConsole(IEnumerable<ConsoleCommand> commands)
	{
		this.commands = commands;
	}

	public CommandResponse ProcessCommand(string inputValue)
	{
		string[] array = inputValue.Split(' ');
		string commandInput = array[0];
		string[] args = array.Skip(1).ToArray();
		return ProcessCommand(commandInput, args);
	}

	private static string CommonPrefix(string a, string b)
	{
		int num = Mathf.Min(a.Length, b.Length);
		for (int i = 0; i < num; i++)
		{
			if (a[i] != b[i])
			{
				return a.Substring(0, i);
			}
		}
		return a.Substring(0, num);
	}

	public string Autocomplete(string input)
	{
		List<string> list = new List<string>();
		foreach (ConsoleCommand command in commands)
		{
			if (command.CommandWord.ToLower().StartsWith(input))
			{
				if (list.Count >= 1)
				{
					list[0] = CommonPrefix(list[0].ToString(), command.CommandWord.ToString());
				}
				else
				{
					list.Add(command.CommandWord);
				}
			}
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		return null;
	}

	public CommandResponse ProcessCommand(string commandInput, string[] args)
	{
		foreach (ConsoleCommand command in commands)
		{
			if (commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
			{
				hasBeenUsedThisSession = true;
				try
				{
					return command.Process(args);
				}
				catch (Exception ex)
				{
					return new CommandResponse(success: false, "Error executing command: " + ex.Message + "\n" + ex.StackTrace);
				}
			}
		}
		return null;
	}

	public IEnumerable<ConsoleCommand> GetAllCommands()
	{
		return commands;
	}
}
