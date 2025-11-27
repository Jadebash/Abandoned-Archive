using UnityEngine;

public class Destroy : MonoBehaviour
{
	private void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}
}
