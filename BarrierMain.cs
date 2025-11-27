using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Barrier Main")]
public class BarrierMain : Spell
{
	public override void ApplyUpgrade(int upgradeIndex)
	{
		throw new NotImplementedException();
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, targetPoint, Quaternion.identity);
		Vector3 vector = targetPoint - player.transform.position;
		float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
	}
}
