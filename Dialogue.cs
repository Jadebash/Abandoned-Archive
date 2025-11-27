using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
	public DialogueSpeaker speaker;

	public List<LocalisedString> dialogue;

	private Queue<LocalisedString> dialogueQueue;

	public bool isEmpty()
	{
		if (dialogueQueue == null)
		{
			return false;
		}
		if (dialogueQueue.Count == 0)
		{
			dialogueQueue = null;
			return true;
		}
		return false;
	}

	public LocalisedString GetNextLine()
	{
		if (dialogueQueue == null)
		{
			dialogueQueue = new Queue<LocalisedString>();
			MakeQueue();
		}
		return dialogueQueue.Dequeue();
	}

	private void MakeQueue()
	{
		foreach (LocalisedString item in dialogue)
		{
			dialogueQueue.Enqueue(item);
		}
	}
}
