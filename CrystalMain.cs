using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Crystal Main")]
public class CrystalMain : Spell
{
	private GameObject prefabInstance;

	private float throwForce;

	private bool charging;

	private bool fired;

	private Vector3 targetPos;

	[HideInInspector]
	public float throwForceScale = 1f;

	[HideInInspector]
	public int shrapnelCount = 12;

	[HideInInspector]
	public float damageScale = 1f;

	[HideInInspector]
	public float bleedScale = 3f;

	private GameObject player;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			damageScale *= 1.35f;
			shrapnelCount--;
			break;
		case 2:
			throwForceScale *= 1.3f;
			break;
		case 3:
			cooldownTime *= 0.8f;
			break;
		case 4:
			bleedScale *= 1.3f;
			break;
		case 5:
			shrapnelCount += 2;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		this.player = player;
		Screenshake.Instance.AddTrauma(Time.deltaTime * 2f);
		targetPos = targetPoint;
		if (prefabInstance != null)
		{
			prefabInstance.transform.up = targetPoint - prefabInstance.transform.position;
		}
		if (!charging)
		{
			prefabInstance = Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
			charging = true;
		}
		if (charging)
		{
			throwForce += Time.deltaTime * throwForceScale * 2f;
		}
		chargeTimer += Time.deltaTime;
		if (chargeTimer >= 1.5f || (chargeTimer >= 0.5f && releasedEarly))
		{
			fired = false;
			charging = false;
			Charged();
		}
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		if (chargeTimer < 0.5f && !clear)
		{
			releasedEarly = true;
			Execute(player, damageMultiplier);
			return;
		}
		if (!fired)
		{
			Vector3 vector = targetPos - prefabInstance.transform.position;
			prefabInstance.GetComponent<Rigidbody2D>().AddForce(vector.normalized * throwForce * 4f * (chargeTimer / 2f), ForceMode2D.Impulse);
			prefabInstance.GetComponent<CrystalOrb>().thrown = true;
			prefabInstance.GetComponent<CrystalOrb>().shrapnelCount = shrapnelCount;
			prefabInstance.GetComponent<CrystalOrb>().damageScale = damageScale;
			prefabInstance.GetComponent<CrystalOrb>().bleedScale = bleedScale;
			CastAnimationOneHanded(player);
			charging = false;
			throwForce = 0f;
		}
		base.Charged();
		releasedEarly = false;
	}
}
