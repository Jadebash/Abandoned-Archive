using UnityEngine;

public class TriggerDirectionIndicator : MonoBehaviour
{
	public Transform target;

	public Color color = Color.red;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			DirectionIndicators.Instance.AddIndicator(target, color);
		}
	}
}
