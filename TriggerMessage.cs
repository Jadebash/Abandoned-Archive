using UnityEngine;

public class TriggerMessage : MonoBehaviour
{
	public bool onlyPlayer = true;

	public bool animationTrigger;

	public string message;

	public GameObject target;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if ((onlyPlayer && other.tag == "Player") || !onlyPlayer)
		{
			if (animationTrigger)
			{
				target.GetComponent<Animator>().SetTrigger(message);
			}
			else
			{
				target.SendMessage(message);
			}
		}
	}
}
