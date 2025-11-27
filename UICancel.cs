using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UICancel : MonoBehaviour, ICancelHandler, IEventSystemHandler
{
	public GameObject disableObject;

	public GameObject enableObject;

	public string unloadScene = "";

	public GameObject sendMessageRecipient;

	public string message;

	public GameObject select;

	public void OnCancel(BaseEventData data)
	{
		DoCancel();
	}

	private void DoCancel()
	{
		if (base.gameObject.activeSelf)
		{
			RuntimeManager.PlayOneShot("event:/SFX/UI/Click", base.transform.position);
			if (sendMessageRecipient != null)
			{
				sendMessageRecipient.SendMessage(message);
			}
			if (disableObject != null)
			{
				disableObject.SetActive(value: false);
			}
			if (enableObject != null)
			{
				enableObject.SetActive(value: true);
			}
			if (select != null)
			{
				EventSystem.current.SetSelectedGameObject(select);
			}
			if (unloadScene != "")
			{
				SceneManager.UnloadSceneAsync(unloadScene);
			}
		}
	}
}
