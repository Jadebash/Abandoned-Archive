using UnityEngine;

public class RelicUIInstance : MonoBehaviour
{
	public void StartHover()
	{
		RelicCollector.Instance.ShowRelicTooltip(base.gameObject);
	}

	public void EndHover()
	{
		RelicCollector.Instance.HideRelicTooltip();
	}
}
