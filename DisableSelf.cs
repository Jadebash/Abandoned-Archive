using UnityEngine;

public class DisableSelf : MonoBehaviour
{
	public void Disable()
	{
		base.gameObject.SetActive(value: false);
	}
}
