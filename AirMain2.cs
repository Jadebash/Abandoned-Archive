using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Air Main 2")]
public class AirMain2 : Spell
{
	private GameObject prefabInstance;

	private GameObject playerInstance;

	private Vector3 curTarget;

	private float damageModifier = 1f;

	private float projectileSpeed = 9f;

	private bool doDamageMod;

	[HideInInspector]
	public int damageMods;

	[HideInInspector]
	public float chargeMultiplier = 1f;

	[HideInInspector]
	public int allowedBounces = 2;

	private float doubleBounceChance;

	private bool hookedToFloorManager;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			allowedBounces += 3;
			damageModifier *= 0.8f;
			break;
		case 2:
			allowedBounces += 2;
			break;
		case 3:
		{
			cooldownTime += 2f;
			if (doubleBounceChance == 0f)
			{
				doubleBounceChance = 0.4f;
				break;
			}
			float num = (1f - doubleBounceChance) * 0.4f;
			doubleBounceChance += num;
			break;
		}
		case 4:
			projectileSpeed += 9f;
			damageModifier *= 1.2f;
			break;
		case 5:
			doDamageMod = true;
			break;
		case 6:
			allowedBounces++;
			cooldownTime *= 0.8f;
			projectileSpeed += 2f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageModifier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		if (!hookedToFloorManager && FloorManager.Instance != null)
		{
			FloorManager.Instance.OnStartFloor += ResetDamageMod;
			hookedToFloorManager = true;
		}
		playerInstance = player;
		curTarget = targetPoint;
		if (prefabInstance == null)
		{
			prefabInstance = Object.Instantiate(prefab, player.transform);
			Screenshake.Instance.AddTrauma(0.4f);
			prefabInstance.GetComponent<CircleCollider2D>().enabled = false;
			prefabInstance.GetComponent<DiskCyclone>().enabled = false;
			Screenshake.Instance.AddTrauma(0.3f);
		}
		Screenshake.Instance.AddTrauma(Time.deltaTime * 1.5f);
		chargeTimer += Time.deltaTime * chargeMultiplier;
		prefabInstance.transform.position = player.transform.position;
		float num = Mathf.Clamp(chargeTimer, 0.5f, 1f);
		prefabInstance.transform.localScale = new Vector3(num, num, 1f);
		if (chargeTimer >= 1f || (chargeTimer > 0.5f && releasedEarly))
		{
			releasedEarly = false;
			Charged(damageModifier);
		}
	}

	public override void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		if (chargeTimer < 0.5f && !clear)
		{
			releasedEarly = true;
			Execute(playerInstance, damageMultiplier);
			return;
		}
		_ = curTarget;
		DiskCyclone component = prefabInstance.GetComponent<DiskCyclone>();
		DamageOnEnter component2 = prefabInstance.GetComponent<DamageOnEnter>();
		component2.damage *= damageModifier * damageMultiplier * chargeTimer * 1.5f;
		component2.OnKill += killedEnemy;
		component.player = playerInstance;
		component.projSpeed = projectileSpeed;
		int num = allowedBounces;
		if (doubleBounceChance > 0f && Random.value < doubleBounceChance)
		{
			num *= 2;
		}
		component.bounces = num;
		component.target = curTarget;
		component.transform.parent = null;
		component.thrown = true;
		prefabInstance.GetComponent<CircleCollider2D>().enabled = true;
		component.SetTarget(curTarget);
		component.enabled = true;
		Screenshake.Instance.AddTrauma(chargeTimer * 0.8f);
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Water_Main_Throw", prefabInstance.transform.position);
		base.Charged();
		prefabInstance = null;
	}

	public void killedEnemy(GameObject enemy)
	{
		if (doDamageMod && damageMods <= 40)
		{
			damageMods++;
			damageModifier += 0.03f;
		}
	}

	private void ResetDamageMod(int floorNum)
	{
		damageModifier -= 0.03f * (float)damageMods;
		damageMods = 0;
	}

	private void OnDestroy()
	{
		if (FloorManager.Instance != null)
		{
			FloorManager.Instance.OnStartFloor -= ResetDamageMod;
		}
	}
}
