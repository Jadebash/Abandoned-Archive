using UnityEngine;

public class GiveCartography : MonoBehaviour
{
	public Relic cartography;

	private void Start()
	{
		RelicCollector.Instance.GetRelic(cartography);
	}
}
