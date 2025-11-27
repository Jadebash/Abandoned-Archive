using UnityEngine;

public class Follow : MonoBehaviour
{
	public Transform target;

	private void Update()
	{
		if (target != null)
		{
			base.transform.position = target.position;
		}
	}
}
