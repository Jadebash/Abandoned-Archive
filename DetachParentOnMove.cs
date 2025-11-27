using UnityEngine;

public class DetachParentOnMove : MonoBehaviour
{
	private Vector3 lastPosition;

	private bool hasDetached;

	[SerializeField]
	private float movementThreshold = 0.001f;

	private void Start()
	{
		lastPosition = base.transform.position;
	}

	private void Update()
	{
		if (!hasDetached && base.transform.parent != null && Vector3.Distance(base.transform.position, lastPosition) > movementThreshold)
		{
			base.transform.parent = null;
			hasDetached = true;
		}
	}
}
