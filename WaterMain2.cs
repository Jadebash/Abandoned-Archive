using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Water Main 2")]
public class WaterMain2 : Spell
{
	private GameObject prefabInstance;

	public float maxLength = 2f;

	public int damagePerHit = 1;

	public float gapTime = 1f;

	public float timeToDamage = 10f;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
			return;
		}
		Debug.LogWarning("Upgrade out of bounds.");
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		chargeTimer += Time.deltaTime;
		Screenshake.Instance.AddTrauma(Time.deltaTime * 2f);
		if (prefabInstance == null)
		{
			prefabInstance = Object.Instantiate(prefab, player.transform);
		}
		prefabInstance.transform.rotation = Quaternion.Slerp(prefabInstance.transform.rotation, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, targetPoint - player.transform.position, Vector3.forward) + 90f, Vector3.forward), Time.deltaTime * 2.5f);
		prefabInstance.GetComponent<SwampsDregs>().player = player;
		if (chargeTimer > 3f)
		{
			Charged(damageMultiplier);
		}
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		base.Charged();
		Object.Destroy(prefabInstance);
		prefabInstance = null;
	}
}
