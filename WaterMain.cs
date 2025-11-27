using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Water Main")]
public class WaterMain : Spell
{
	private GameObject prefabInstance;

	private EventInstance ChargeUpSound;

	public GameObject chargeUpParticles;

	private ParticleSystem chargeUpParticlesInstance;

	private GameObject playerInstance;

	[HideInInspector]
	public float forceMultiplier = 1f;

	[HideInInspector]
	public float chargeMultiplier = 1f;

	[HideInInspector]
	public int pierce;

	private GameObject lineSprite;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			forceMultiplier *= 1.5f;
			chargeMultiplier *= 1.1f;
			break;
		case 2:
			chargeMultiplier *= 1.4f;
			cooldownTime *= 1.25f;
			break;
		case 3:
			damage *= 1.3f;
			chargeMultiplier *= 0.75f;
			cooldownTime *= 1.25f;
			break;
		case 4:
			pierce++;
			break;
		case 5:
			cooldownTime *= 0.75f;
			damage *= 0.9f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		playerInstance = player;
		GameObject gameObject = playerInstance.transform.Find("ShotIndicator").gameObject;
		lineSprite = gameObject.transform.Find("LineSprite").gameObject;
		lineSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 255f);
		chargeTimer += Time.deltaTime * chargeMultiplier;
		Screenshake.Instance.AddTrauma(Time.deltaTime);
		if (prefabInstance == null)
		{
			prefabInstance = Object.Instantiate(prefab);
			ChargeUpSound = RuntimeManager.CreateInstance("event:/SFX/Main Character/Spells/Water_Main_Charge");
			ChargeUpSound.start();
			Screenshake.Instance.AddTrauma(0.1f);
			prefabInstance.transform.GetChild(1).gameObject.SetActive(value: false);
			chargeUpParticlesInstance = Object.Instantiate(chargeUpParticles, player.transform.position, Quaternion.identity, player.transform).transform.GetChild(0).GetComponent<ParticleSystem>();
		}
		float num = Mathf.Clamp(chargeTimer, 0.5f, 1f);
		prefabInstance.transform.localScale = new Vector3(num, num, 1f);
		prefabInstance.GetComponent<Rigidbody2D>().simulated = false;
		prefabInstance.transform.position = player.transform.position;
		prefabInstance.transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, targetPoint - player.transform.position, Vector3.forward), Vector3.forward);
		gameObject.transform.position = player.transform.position;
		gameObject.transform.localScale = new Vector3(num * 2f, 1f, 1f);
		gameObject.transform.rotation = prefabInstance.transform.rotation * Quaternion.Euler(new Vector3(1f, 1f, 90f));
		if (chargeTimer >= 1f || (chargeTimer >= 0.5f && releasedEarly))
		{
			Charged(damageMultiplier);
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
		lineSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
		float num = Mathf.Clamp(chargeTimer, 0f, 1f);
		if (pierce > 0)
		{
			prefabInstance.GetComponent<StickOnto>().pierce = pierce;
		}
		prefabInstance.GetComponent<StickOnto>().damage = Mathf.Round(damage * num * damageMultiplier);
		prefabInstance.GetComponent<StickOnto>().forceMultiplier = forceMultiplier;
		chargeUpParticlesInstance.Stop();
		base.Charged();
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Water_Main_Throw", prefabInstance.transform.position);
		prefabInstance.GetComponent<Rigidbody2D>().simulated = true;
		prefabInstance.transform.position = playerInstance.transform.position;
		prefabInstance.transform.GetChild(1).gameObject.SetActive(value: true);
		prefabInstance.GetComponent<Rigidbody2D>().AddForce(prefabInstance.transform.up * 25f * forceMultiplier, ForceMode2D.Impulse);
		Screenshake.Instance.AddTrauma(0.3f);
		playerInstance.GetComponent<Movement>().anim.SetTrigger("castOneHand");
		ChargeUpSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		prefabInstance = null;
		releasedEarly = false;
	}
}
