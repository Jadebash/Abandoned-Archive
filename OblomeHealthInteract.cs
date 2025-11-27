using System.Collections;
using UnityEngine;

public class OblomeHealthInteract : Interactable
{
	public int cost;

	public GameObject upgradeParticlesPrefab;

	public Dialogue firstDialogue;

	public Dialogue standardDialogue;

	public Dialogue secondDialogue;

	public Dialogue fullHealthDialogue;

	public GameObject healthModal;

	public bool thinking;

	public Dialogue thinkingDialogue;

	public bool sameFloor;

	private float healthInteractTimer = 2f;

	public bool exitedHealthModal;

	public bool healthInteractable = true;

	private bool transitioningFromDialogue;

	private void Start()
	{
		Debug.Log("OblomeHealthInteract Start: sameFloor = " + sameFloor);
		if (FloorManager.Instance != null)
		{
			FloorManager.Instance.OnStartFloor += OnFloorChanged;
		}
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

	private void OnDestroy()
	{
		if (FloorManager.Instance != null)
		{
			FloorManager.Instance.OnStartFloor -= OnFloorChanged;
		}
	}

	private void OnFloorChanged(int floorNumber)
	{
		sameFloor = false;
		Debug.Log("Floor changed to " + floorNumber + ", sameFloor reset to false");
	}

	public void Pay()
	{
		if (!healthInteractable || transitioningFromDialogue)
		{
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
		if (!SaveManager.Instance.currentSave.oblomeHealthInteract)
		{
			if (!thinking)
			{
				DialogueSystem.Instance.StartDialogue(firstDialogue);
				DialogueSystem.Instance.OnFinishDialogue += OnFirstDialogueFinished;
				thinking = true;
			}
			else
			{
				healthModal.SetActive(value: true);
				Manager.Instance?.PauseGame();
				MusicManager.EnterMenu();
			}
		}
		else if (sameFloor)
		{
			DialogueSystem.Instance.StartDialogue(secondDialogue);
			DialogueSystem.Instance.OnFinishDialogue += OnSecondDialogueFinished;
		}
		else
		{
			DialogueSystem.Instance.StartDialogue(standardDialogue);
			DialogueSystem.Instance.OnFinishDialogue += OnStandardDialogueFinished;
		}
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

	private void OnStandardDialogueFinished(Dialogue dialogue)
	{
		Debug.Log("OnStandardDialogueFinished");
		DialogueSystem.Instance.OnFinishDialogue -= OnStandardDialogueFinished;
		transitioningFromDialogue = true;
		StartCoroutine(OpenHealthModalAfterDelay());
	}

	private void OnSecondDialogueFinished(Dialogue dialogue)
	{
		Debug.Log("OnSecondDialogueFinished");
		DialogueSystem.Instance.OnFinishDialogue -= OnSecondDialogueFinished;
		exitedHealthModal = true;
		healthInteractable = false;
	}

	private void OnFullHealthDialogueFinished(Dialogue dialogue)
	{
		Debug.Log("OnFullHealthDialogueFinished");
		DialogueSystem.Instance.OnFinishDialogue -= OnFullHealthDialogueFinished;
		exitedHealthModal = true;
		healthInteractable = false;
	}
}
