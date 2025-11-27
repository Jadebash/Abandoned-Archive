using UnityEngine;

public class RoomWalkTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			WaveManager.Instance?.WalkedRoom(base.transform.parent);
		}
	}
}
