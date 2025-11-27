using UnityEngine;

public class DisableTooltip : MonoBehaviour
{
	private void OnDisable()
	{
		base.gameObject.SetActive(value: false);
	}
}
