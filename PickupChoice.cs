using System.Collections.Generic;
using UnityEngine;

public class PickupChoice : MonoBehaviour
{
	public delegate void ChoiceMadeCallback(Interactable choice);

	private List<Interactable> choices = new List<Interactable>();

	public bool waitForUnpause;

	private Manager.Pause pauseCallback;

	public event ChoiceMadeCallback OnChoiceMade;

	private void Start()
	{
		foreach (Transform item in base.transform)
		{
			Interactable component = item.GetComponent<Interactable>();
			if (component != null)
			{
				choices.Add(component);
				component.OnInteract += Interacted;
			}
		}
	}

	public void Interacted(Interactable pickup, GameObject player)
	{
		if (waitForUnpause)
		{
			if (pauseCallback != null)
			{
				Manager.Instance.OnPause -= pauseCallback;
			}
			pauseCallback = delegate(bool paused)
			{
				if (!paused)
				{
					Debug.Log("Made choice");
					Manager.Instance.OnPause -= pauseCallback;
					pauseCallback = null;
					MadeChoice(pickup, player);
				}
			};
			Manager.Instance.OnPause += pauseCallback;
		}
		else
		{
			MadeChoice(pickup, player);
		}
	}

	private void OnDestroy()
	{
		if (pauseCallback != null && Manager.Instance != null)
		{
			Manager.Instance.OnPause -= pauseCallback;
			pauseCallback = null;
		}
		foreach (Interactable choice in choices)
		{
			if (choice != null)
			{
				choice.OnInteract -= Interacted;
			}
		}
	}

	public void MadeChoice(Interactable pickup, GameObject player)
	{
		if (!(pickup != null) || !(pickup.gameObject != null))
		{
			return;
		}
		pickup.OnInteract -= Interacted;
		pickup.transform.parent = null;
		Interactable[] array = choices.ToArray();
		foreach (Interactable interactable in array)
		{
			if (interactable != null && pickup != interactable)
			{
				Object.Destroy(interactable.gameObject);
			}
		}
		this.OnChoiceMade?.Invoke(pickup);
		pickup = null;
	}
}
