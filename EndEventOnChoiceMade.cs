using UnityEngine;

[RequireComponent(typeof(PickupChoice))]
public class EndEventOnChoiceMade : MonoBehaviour
{
	private void Start()
	{
		GetComponent<PickupChoice>().OnChoiceMade += ChoiceMade;
	}

	public void ChoiceMade(Interactable choice)
	{
		Debug.Log("event ended");
		GetComponentInParent<EventRoom>().EndEvent();
	}
}
