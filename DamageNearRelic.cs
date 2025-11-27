using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Damage Near Relic")]
public class DamageNearRelic : Relic
{
	public GameObject prefab;

	private GameObject instance;

	public override void GainedRelic(GameObject player)
	{
		instance = Object.Instantiate(prefab, player.transform);
		Use();
	}

	public override void LostRelic(GameObject player)
	{
		Object.Destroy(instance);
	}
}
