using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Sun Main")]
public class SunMain : Spell
{
	private GameObject prefabInstance;

	private EventInstance ChargeUpSound;

	private GameObject player;

	public GameObject stunExplosionEffectPrefab;

	[HideInInspector]
	public float xScale = 1f;

	[HideInInspector]
	public float movementSpeedModifier;

	[HideInInspector]
	public bool stunEnemiesOnKill;

	[HideInInspector]
	public float stunTime;

	[HideInInspector]
	public bool pierceEnemies;

	public float slerpSpeed = 2.5f;

	public LayerMask raycastLayerMaskPierce;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			damage *= 1.5f;
			movementSpeedModifier -= 0.5f * (1f + movementSpeedModifier);
			cooldownTime *= 1.2f;
			break;
		case 2:
			pierceEnemies = true;
			cooldownTime *= 1.2f;
			break;
		case 3:
			cooldownTime *= 0.7f;
			break;
		case 4:
			stunEnemiesOnKill = true;
			stunTime += 1f;
			break;
		case 5:
			xScale += 0.25f;
			cooldownTime *= 1.1f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		chargeTimer += Time.deltaTime;
		Screenshake.Instance.AddTrauma(Time.deltaTime * 1.5f);
		this.player = player;
		if (prefabInstance == null)
		{
			ChargeUpSound = RuntimeManager.CreateInstance("event:/SFX/Main Character/Spells/Sun_Main");
			ChargeUpSound.start();
			prefabInstance = Object.Instantiate(prefab, player.transform);
			prefabInstance.transform.Find("Damage").GetComponent<DamageOverTime>().damagePerSecond = damage * damageMultiplier;
			if (pierceEnemies)
			{
				prefabInstance.GetComponent<RaycastLine>().raycastLayerMask = raycastLayerMaskPierce;
			}
			prefabInstance.GetComponent<LineRenderer>().widthMultiplier = 0.4f * xScale;
			prefabInstance.transform.localScale = new Vector3(1f, xScale, 1f);
			if (stunEnemiesOnKill)
			{
				prefabInstance.transform.Find("Damage").GetComponent<DamageOverTime>().OnKill += SpawnExplosion;
				prefabInstance.transform.Find("Damage").GetComponent<DamageOnEnter>().OnKill += SpawnExplosion;
			}
			Screenshake.Instance.AddTrauma(0.6f);
			player.GetComponent<Movement>().speedModifier += movementSpeedModifier;
			this.player = player;
			prefabInstance.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, targetPoint - player.transform.position, Vector3.forward) + 90f, Vector3.forward);
		}
		Vector3[] array = new Vector3[2]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		prefabInstance.GetComponent<LineRenderer>().GetPositions(array);
		float sqrMagnitude = (array[1] - array[0]).sqrMagnitude;
		sqrMagnitude = Mathf.Sqrt(sqrMagnitude);
		prefabInstance.transform.Find("Damage").GetComponent<BoxCollider2D>().offset = new Vector2(sqrMagnitude / 2f, 0f);
		prefabInstance.transform.Find("Damage").GetComponent<BoxCollider2D>().size = new Vector2(sqrMagnitude, 0.25f);
		prefabInstance.transform.rotation = Quaternion.Slerp(prefabInstance.transform.rotation, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, targetPoint - player.transform.position, Vector3.forward) + 90f, Vector3.forward), Time.deltaTime * slerpSpeed);
		if (chargeTimer > 3f || (chargeTimer > 1f && releasedEarly))
		{
			releasedEarly = false;
			Charged(damageMultiplier);
		}
	}

	public void SpawnExplosion(GameObject enemy)
	{
		Object.Instantiate(stunExplosionEffectPrefab, enemy.transform.position, Quaternion.identity);
		Enemy[] array = Object.FindObjectsOfType<Enemy>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stun(stunTime);
		}
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		if (chargeTimer < 1f && !clear)
		{
			releasedEarly = true;
			Execute(player, damageMultiplier);
			return;
		}
		base.Charged();
		Health[] array = prefabInstance.transform.Find("Damage").GetComponent<DamageOverTime>().toDamage.ToArray();
		foreach (Health health in array)
		{
			if (health != null && health.Damage(Mathf.Round(damage * damageMultiplier / 2f)))
			{
				SpawnExplosion(health.gameObject);
			}
		}
		player.GetComponent<Movement>().speedModifier -= movementSpeedModifier;
		ChargeUpSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		Object.Destroy(prefabInstance);
		prefabInstance = null;
		releasedEarly = false;
	}
}
