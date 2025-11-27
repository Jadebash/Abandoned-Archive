using UnityEngine;

public class PropDestroyer : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "PropBreakable" && (bool)other.GetComponent<PropBreak>())
		{
			other.GetComponent<PropBreak>().Break();
		}
	}
}
