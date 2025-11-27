using System.Collections;
using UnityEngine;

public class EndlessPortalInteract : Interactable
{
	[Header("Loop Interaction")]
	public Dialogue loopDialogue;

	public LoopConfirmationModal confirmationModal;

	private bool awaitingDialogue;

	private bool awaitingModalResponse;

	private void OnDisable()
	{
		if (awaitingDialogue && DialogueSystem.Instance != null)
		{
			DialogueSystem.Instance.OnFinishDialogue -= OnLoopDialogueFinished;
		}
		awaitingDialogue = false;
	}

	public void Loop()
	{
		if (isInteractable && !awaitingDialogue && !awaitingModalResponse)
		{
			isInteractable = false;
			if (loopDialogue != null && DialogueSystem.Instance != null)
			{
				awaitingDialogue = true;
				DialogueSystem.Instance.OnFinishDialogue += OnLoopDialogueFinished;
				DialogueSystem.Instance.StartDialogue(loopDialogue);
			}
			else
			{
				ShowConfirmationModal();
			}
		}
	}

	private void OnLoopDialogueFinished(Dialogue dialogue)
	{
		if (!(dialogue != loopDialogue))
		{
			DialogueSystem.Instance.OnFinishDialogue -= OnLoopDialogueFinished;
			awaitingDialogue = false;
			StartCoroutine(ShowConfirmationModalAfterDelay());
		}
	}

	private IEnumerator ShowConfirmationModalAfterDelay()
	{
		yield return new WaitForEndOfFrame();
		ShowConfirmationModal();
	}

	private void ShowConfirmationModal()
	{
		awaitingModalResponse = true;
		if (confirmationModal != null)
		{
			confirmationModal.gameObject.SetActive(value: true);
			confirmationModal.Show(this);
		}
		else
		{
			CompleteLoop();
		}
	}

	public void HandleLoopSelection(bool confirmed)
	{
		StartCoroutine(HandleLoopSelectionRoutine(confirmed));
	}

	private IEnumerator HandleLoopSelectionRoutine(bool confirmed)
	{
		if (confirmationModal != null)
		{
			confirmationModal.gameObject.SetActive(value: false);
		}
		if (confirmed)
		{
			CompleteLoop();
		}
		else
		{
			CancelLoop();
		}
		Manager.Instance?.UnpauseGame();
		MusicManager.ExitMenu();
		yield return new WaitForSeconds(1f);
		awaitingModalResponse = false;
	}

	public void CompleteLoop()
	{
		isInteractable = false;
		FloorManager.Instance.NextFloor();
	}

	public void CancelLoop()
	{
		isInteractable = true;
	}
}
