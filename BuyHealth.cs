using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyHealth : MonoBehaviour
{
	private void Start()
	{
		AssignHealthModal();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		StartCoroutine(DelayedAssignHealthModal());
	}

	private IEnumerator DelayedAssignHealthModal()
	{
		yield return null;
		AssignHealthModal();
	}

	private void AssignHealthModal()
	{
		OblomeHealthInteract oblomeHealthInteract = Object.FindObjectOfType<OblomeHealthInteract>();
		if (oblomeHealthInteract != null)
		{
			oblomeHealthInteract.healthModal = base.transform.GetChild(0).gameObject;
			Debug.Log("Health modal assigned to OblomeHealthInteract");
			return;
		}
		OblomeFinalFloorInteract oblomeFinalFloorInteract = Object.FindObjectOfType<OblomeFinalFloorInteract>();
		if (oblomeFinalFloorInteract != null)
		{
			oblomeFinalFloorInteract.healthModal = base.transform.GetChild(0).gameObject;
			Debug.Log("Health modal assigned to OblomeFinalFloorInteract");
		}
		else if (SceneManager.GetActiveScene().name != "Menu")
		{
			Debug.LogWarning("Neither OblomeHealthInteract nor OblomeFinalFloorInteract found when trying to assign health modal");
		}
	}

	public void RequestModalAssignment()
	{
		AssignHealthModal();
	}

	private void Update()
	{
	}
}
