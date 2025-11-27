using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoopConfirmationModal : MonoBehaviour
{
	[Header("UI References")]
	public Button confirmButton;

	public Button cancelButton;

	public GameObject defaultSelected;

	private EndlessPortalInteract currentPortal;

	private void Awake()
	{
		if (confirmButton != null)
		{
			confirmButton.onClick.AddListener(ConfirmLoop);
		}
		if (cancelButton != null)
		{
			cancelButton.onClick.AddListener(CancelLoop);
		}
	}

	public void Show(EndlessPortalInteract portal)
	{
		currentPortal = portal;
		Manager.Instance?.PauseGame();
		MusicManager.EnterMenu();
		SelectDefault();
	}

	private void SelectDefault()
	{
		GameObject gameObject = defaultSelected ?? ((confirmButton != null) ? confirmButton.gameObject : null);
		if (gameObject != null && EventSystem.current != null)
		{
			EventSystem.current.SetSelectedGameObject(gameObject);
		}
	}

	public void ConfirmLoop()
	{
		currentPortal.HandleLoopSelection(confirmed: true);
	}

	public void CancelLoop()
	{
		currentPortal.HandleLoopSelection(confirmed: false);
	}
}
