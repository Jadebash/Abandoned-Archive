using System.Collections;
using FMODUnity;
using UnityEngine;

public class FinalFloor : MonoBehaviour
{
	[Header("Dialogue")]
	public Dialogue d1;

	public Dialogue d2;

	public Dialogue d3;

	public Dialogue d4;

	public Dialogue d5;

	public Dialogue d6;

	public Dialogue d7;

	[Header("Bosses")]
	public GameObject OblomeBoss;

	public GameObject MysteriousVoiceBoss;

	public RoomInfo leftBossRoom;

	public RoomInfo rightBossRoom;

	[Header("Music")]
	public EventReference finalFloorMusicEvent;

	private float dialogueCounter;

	private GameObject player;

	private float textTriggerPos;

	private void Start()
	{
		textTriggerPos = 5f;
		player = PlayerManager.ClosestPlayer(base.transform.position);
		MusicManager.Instance?.PlaySong(finalFloorMusicEvent);
	}

	private IEnumerator introDialogue()
	{
		yield return new WaitForSeconds(2f);
		DialogueSystem.Instance.StartDialogue(d1);
	}

	private void Update()
	{
		if (dialogueCounter == 6f || !(player != null) || !(player.transform.position.y >= textTriggerPos))
		{
			return;
		}
		textTriggerPos += 5f;
		float num = dialogueCounter;
		if (num <= 2f)
		{
			if (num != 0f)
			{
				if (num != 1f)
				{
					if (num == 2f)
					{
						DialogueSystem.Instance.StartDialogue(d4);
						dialogueCounter += 1f;
					}
				}
				else
				{
					DialogueSystem.Instance.StartDialogue(d3);
					dialogueCounter += 1f;
				}
			}
			else
			{
				DialogueSystem.Instance.StartDialogue(d2);
				dialogueCounter += 1f;
			}
		}
		else if (num != 3f)
		{
			if (num != 4f)
			{
				if (num == 5f)
				{
					DialogueSystem.Instance.StartDialogue(d7);
					dialogueCounter += 1f;
				}
			}
			else
			{
				DialogueSystem.Instance.StartDialogue(d6);
				dialogueCounter += 1f;
			}
		}
		else
		{
			DialogueSystem.Instance.StartDialogue(d5);
			dialogueCounter += 1f;
		}
	}
}
