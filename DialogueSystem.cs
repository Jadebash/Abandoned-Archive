using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
	public delegate void DialogueCallback(Dialogue dialogue);

	public static DialogueSystem Instance;

	private PlayerInput playerInput;

	[Header("UI")]
	public GameObject dialogueUIPanel;

	public TextMeshProUGUI dialogueText;

	public TextMeshProUGUI nameText;

	public Image portraitImage;

	public GameObject prompt;

	[Header("Debug")]
	public bool doDebugDialogue;

	public Dialogue debugDialogue;

	private Dialogue currentDialogue;

	[HideInInspector]
	public bool inDialogue;

	private bool animatingText;

	private bool skippingText;

	private Movement movement;

	private SpellCasting spellCasting;

	public event DialogueCallback OnFinishDialogue;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (doDebugDialogue)
		{
			StartDialogue(debugDialogue);
		}
		movement = Object.FindObjectOfType<Movement>();
		spellCasting = Object.FindObjectOfType<SpellCasting>();
		playerInput = Object.FindObjectOfType<PlayerInput>();
		if (playerInput == null)
		{
			Debug.LogWarning("DialogueSystem cannot find PlayerInput");
		}
	}

	public void StartDialogue(Dialogue dialogue)
	{
		if (!inDialogue)
		{
			currentDialogue = dialogue;
			nameText.text = currentDialogue.speaker.name.value;
			portraitImage.sprite = currentDialogue.speaker.portrait;
			prompt.SetActive(value: false);
			inDialogue = true;
			movement.enabled = false;
			movement.rolling = false;
			movement.anim.SetBool("rolling", value: false);
			movement.SetTargetVelocity(Vector2.zero);
			spellCasting.canCast = false;
			dialogueUIPanel.SetActive(value: true);
			NextLine();
		}
	}

	public void EndDialogue()
	{
		if (inDialogue)
		{
			movement.enabled = true;
			spellCasting.canCast = true;
			inDialogue = false;
			dialogueUIPanel.SetActive(value: false);
			this.OnFinishDialogue?.Invoke(currentDialogue);
		}
	}

	private void NextLine()
	{
		prompt.SetActive(value: false);
		if (currentDialogue.isEmpty())
		{
			EndDialogue();
			return;
		}
		LocalisedString nextLine = currentDialogue.GetNextLine();
		StartCoroutine(AnimateText(dialogueText, nextLine.value, 20f));
	}

	private IEnumerator AnimateText(TextMeshProUGUI textBox, string text, float speed)
	{
		if (text == null)
		{
			Debug.LogError("Text is null in AnimateText");
			yield break;
		}
		textBox.text = "";
		animatingText = true;
		Queue<char> charToSay = new Queue<char>();
		foreach (char item in text)
		{
			charToSay.Enqueue(item);
		}
		float charToDraw = 0f;
		while (charToSay.Count != 0)
		{
			charToDraw += Time.deltaTime * speed;
			if (charToDraw >= 1f || skippingText)
			{
				charToDraw -= 1f;
				char c = charToSay.Dequeue();
				switch (c)
				{
				case '[':
				{
					string text2 = "";
					while (charToSay.Peek() != ']')
					{
						text2 += charToSay.Dequeue();
					}
					charToSay.Dequeue();
					try
					{
						string bindingDisplayString = playerInput.actions[text2].GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
						textBox.text += bindingDisplayString;
					}
					catch
					{
						textBox.text += text2;
					}
					continue;
				}
				case '!':
				case ',':
				case '.':
				case ';':
				case '?':
					charToDraw -= 4f;
					break;
				}
				textBox.text += c;
				if (!skippingText && IsEnglishLetter(c))
				{
					RuntimeManager.PlayOneShot(currentDialogue.speaker.sfx);
				}
			}
			else
			{
				yield return null;
			}
		}
		prompt.SetActive(value: true);
		animatingText = false;
		skippingText = false;
	}

	private void CancelTextAnimation()
	{
		StopCoroutine("AnimateText");
	}

	public void DialogueInteraction(InputAction.CallbackContext context)
	{
		if (context.started && Time.timeScale != 0f && inDialogue)
		{
			if (animatingText)
			{
				skippingText = true;
				RuntimeManager.PlayOneShot(currentDialogue.speaker.sfx);
				RuntimeManager.PlayOneShot("event:/SFX/UI/Click");
			}
			else
			{
				NextLine();
				RuntimeManager.PlayOneShot("event:/SFX/UI/Click");
			}
		}
	}

	private bool IsEnglishLetter(char c)
	{
		if (c < 'A' || c > 'Z')
		{
			if (c >= 'a')
			{
				return c <= 'z';
			}
			return false;
		}
		return true;
	}
}
