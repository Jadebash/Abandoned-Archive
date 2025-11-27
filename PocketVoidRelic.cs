using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Pocket Void Relic")]
public class PocketVoidRelic : Relic
{
	public GameObject pocketVoidPrefab;

	public GameObject projectilePrefab;

	public GameObject rechargeEffectPrefab;

	private GameObject pocketVoidInstance;

	public override void GainedRelic(GameObject player)
	{
		pocketVoidInstance = Object.Instantiate(pocketVoidPrefab, player.transform);
		PocketVoid component = pocketVoidInstance.GetComponent<PocketVoid>();
		component.pocketVoidPrefab = projectilePrefab;
		component.rechargeEffectPrefab = rechargeEffectPrefab;
		component.OnDispense += base.Use;
	}

	public override void LostRelic(GameObject player)
	{
		Object.Destroy(pocketVoidInstance);
	}
}
