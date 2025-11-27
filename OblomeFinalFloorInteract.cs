using System.Collections;
using UnityEngine;

public class OblomeFinalFloorInteract : Interactable
{
	public GameObject upgradeParticlesPrefab;

	public Dialogue firstDialogue;

	public Dialogue fullHealthDialogue;

	public GameObject healthModal;

	private float healthInteractTimer = 2f;

	public bool exitedHealthModal;

	public bool healthInteractable = true;

	private bool transitioningFromDialogue;

	private bool spoken;

	public int floor4PurchaseCount;

	private void Start()
	{
		StartCoroutine(EnsureHealthModalAssigned());
	}

	private IEnumerator EnsureHealthModalAssigned()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (healthModal == null)
		{
			Debug.Log("Health modal not assigned, requesting assignment from BuyHealth component");
			BuyHealth buyHealth = Object.FindObjectOfType<BuyHealth>();
			if (buyHealth != null)
			{
				buyHealth.RequestModalAssignment();
			}
			else
			{
				Debug.LogWarning("BuyHealth component not found when trying to assign health modal");
			}
		}
	}

	private void Update()
	{
		if (exitedHealthModal)
		{
			healthInteractTimer -= Time.deltaTime;
			if (healthInteractTimer <= 0f && !healthInteractable)
			{
				healthInteractable = true;
				healthInteractTimer = 0.5f;
				exitedHealthModal = false;
			}
		}
	}

	public void Pay()
	{
		if (!healthInteractable || transitioningFromDialogue)
		{
			return;
		}
		if (floor4PurchaseCount >= 2)
		{
			DialogueSystem.Instance.StartDialogue(fullHealthDialogue);
			DialogueSystem.Instance.OnFinishDialogue += OnFullHealthDialogueFinished;
			return;
		}
		GameObject gameObject = PlayerManager.ClosestPlayer(base.transform.position);
		if (gameObject != null)
		{
			Health component = gameObject.GetComponent<Health>();
			if (component != null && component.health >= component.maxHealth)
			{
				DialogueSystem.Instance.StartDialogue(fullHealthDialogue);
				DialogueSystem.Instance.OnFinishDialogue += OnFullHealthDialogueFinished;
				return;
			}
		}
		DialogueSystem.Instance.StartDialogue(firstDialogue);
		DialogueSystem.Instance.OnFinishDialogue += OnFirstDialogueFinished;
	}

	private void OnFirstDialogueFinished(Dialogue dialogue)
	{
		Debug.Log("OnFirstDialogueFinished");
		DialogueSystem.Instance.OnFinishDialogue -= OnFirstDialogueFinished;
		transitioningFromDialogue = true;
		StartCoroutine(OpenHealthModalAfterDelay());
	}

	private IEnumerator OpenHealthModalAfterDelay()
	{
		yield return new WaitForEndOfFrame();
		healthModal.SetActive(value: true);
		Manager.Instance?.PauseGame();
		MusicManager.EnterMenu();
		transitioningFromDialogue = false;
	}

	private void OnFullHealthDialogueFinished(Dialogue dialogue)
	{
		Debug.Log("OnFullHealthDialogueFinished");
		DialogueSystem.Instance.OnFinishDialogue -= OnFullHealthDialogueFinished;
		exitedHealthModal = true;
		healthInteractable = false;
	}
}
