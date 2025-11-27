using UnityEngine;

public class GreedChoice : MonoBehaviour
{
	public Interactable pickupable;

	public GameObject pickupPrefab;

	private int count = 1;

	private void Start()
	{
		pickupable.OnInteract += GreedPickup;
	}

	private void GreedPickup(Interactable pickup, GameObject player)
	{
		int num = Mathf.Clamp(count * 10, 0, 90);
		if (Random.Range(1, 101) < num && player.GetComponent<Health>().Damage(20f, base.transform.parent.gameObject, ignoreInvincibility: true))
		{
			Steam.TriggerAchievement("ACH_DIE_GREED");
		}
		Object.Instantiate(pickupPrefab, new Vector3(base.transform.position.x + 1.5f, base.transform.position.y, 0f), Quaternion.identity, base.transform).GetComponent<Interactable>().OnInteract += GreedPickup;
		count++;
	}
}
