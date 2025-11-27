using System.Linq;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Void Main")]
public class VoidMain : Spell
{
	private GameObject prefabInstance;

	private GameObject player;

	[HideInInspector]
	private float stunMod = 1f;

	[HideInInspector]
	private float speedMod = 1f;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			cooldownTime *= 0.75f;
			break;
		case 2:
			damage *= 1.2f;
			break;
		case 3:
			stunMod *= 1.1f;
			break;
		case 4:
			speedMod *= 1.5f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		case 5:
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		GameObject.FindGameObjectsWithTag("Enemy").ToList();
		Screenshake.Instance.AddTrauma(0.6f);
		prefabInstance = Object.Instantiate(prefab);
		prefabInstance.transform.position = player.transform.position;
		this.player = player;
		VoidDagger component = prefabInstance.GetComponent<VoidDagger>();
		component.GetComponent<VoidDagger>().moveSpeed *= speedMod;
		component.GetComponent<VoidDagger>().damage = damage;
		component.GetComponent<VoidDagger>().stunTime *= stunMod;
		component.GetComponent<VoidDagger>().from = targetPoint;
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Water_Main_Throw", player.transform.position);
		CastAnimationOneHanded(player);
		Screenshake.Instance.AddTrauma(0.2f);
	}
}
