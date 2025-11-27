using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Earth Main")]
public class EarthMain : Spell
{
	private GameObject prefabInstance;

	private GameObject player;

	[HideInInspector]
	public float fireTimer;

	[HideInInspector]
	public int shotCount;

	[HideInInspector]
	public Vector2 scale = new Vector2(0.65f, 0.65f);

	[HideInInspector]
	public float fireRateScale = 1f;

	[HideInInspector]
	public float speedScale = 5f;

	[HideInInspector]
	public float[] fireDelays = new float[15]
	{
		0.2f, 0.2f, 0.175f, 0.175f, 0.15f, 0.15f, 0.1f, 0.1f, 0.1f, 0.1f,
		0.1f, 0.1f, 0.1f, 0.1f, 0.1f
	};

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			damage *= 1.3f;
			break;
		case 2:
			scale *= 1.15f;
			fireRateScale *= 1.15f;
			speedScale *= 1.15f;
			break;
		case 3:
			scale *= 1.3f;
			break;
		case 4:
			fireRateScale *= 1.3f;
			break;
		case 5:
			speedScale *= 1.3f;
			fireDelays = fireDelays.Append(0.1f).Append(0.1f).ToArray();
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		this.player = player;
		if (fireTimer == 0f && shotCount == 0)
		{
			FireProjectile(targetPoint);
		}
		fireTimer += Time.deltaTime * fireRateScale;
		if (shotCount < fireDelays.Length && fireTimer >= fireDelays[shotCount])
		{
			FireProjectile(targetPoint);
			shotCount++;
			fireTimer = 0f;
		}
		chargeTimer += Time.deltaTime;
		float num = fireDelays.Sum() / fireRateScale;
		if (chargeTimer >= num || (chargeTimer >= 1f && releasedEarly))
		{
			Charged(damageMultiplier);
		}
	}

	private void FireProjectile(Vector3 targetPoint)
	{
		prefabInstance = Object.Instantiate(prefab, player.transform.position, Quaternion.Euler(Vector3.zero));
		prefabInstance.transform.localScale = scale;
		EarthMainSeed component = prefabInstance.GetComponent<EarthMainSeed>();
		component.speed = speedScale;
		prefabInstance.transform.up = targetPoint - prefabInstance.transform.position;
		component.SetDamage(damage);
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		if (chargeTimer < 1f && !clear)
		{
			releasedEarly = true;
			Execute(player, damageMultiplier);
			return;
		}
		fireTimer = 0f;
		shotCount = 0;
		base.Charged();
		releasedEarly = false;
	}
}
