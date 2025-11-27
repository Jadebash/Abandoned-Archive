using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopModal : MonoBehaviour
{
	private void Start()
	{
		AssignLoopModal();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		StartCoroutine(DelayedAssignLoopModal());
	}

	private IEnumerator DelayedAssignLoopModal()
	{
		yield return null;
		AssignLoopModal();
	}

	private void AssignLoopModal()
	{
		EndlessPortalInteract endlessPortalInteract = Object.FindObjectOfType<EndlessPortalInteract>();
		if (endlessPortalInteract != null)
		{
			LoopConfirmationModal component = base.transform.GetChild(0).GetComponent<LoopConfirmationModal>();
			if (component != null)
			{
				endlessPortalInteract.confirmationModal = component;
				Debug.Log("Loop confirmation modal assigned to EndlessPortalInteract");
			}
		}
	}

	public void RequestModalAssignment()
	{
		AssignLoopModal();
	}
}
