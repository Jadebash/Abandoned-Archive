using UnityEngine;

public class DisableColliderOnDestroy : MonoBehaviour
{
	private void OnDestroy()
	{
		GetComponent<Collider2D>().enabled = false;
	}
}
