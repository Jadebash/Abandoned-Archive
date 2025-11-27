using System.Collections;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Crystal Special")]
public class CrystalSpecial : SpecialSpell
{
	private Enemy[] enemies;

	private Animator anim;

	private GameObject playerGO;

	[HideInInspector]
	public int rollShrapnel = 2;

	[HideInInspector]
	public int mainShrapnel = 6;

	[HideInInspector]
	public float bleedScale = 3.5f;

	[HideInInspector]
	public float pushForceScale = 1f;

	[HideInInspector]
	public float distanceScale = 1f;

	public GameObject crystalFists;

	private int rollCounter;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			cooldownTime *= 0.8f;
			break;
		case 2:
			rollShrapnel += 3;
			break;
		case 3:
			mainShrapnel += 3;
			break;
		case 4:
			bleedScale *= 1.3f;
			cooldownTime *= 1.1f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		playerGO = player;
		anim = player.GetComponent<Animator>();
		isCoroutineRunning = true;
		CoroutineEmitter.Instance.StartCoroutine(slamHands());
	}

	public override void Roll(GameObject player)
	{
		if (!GameObject.Find("CrystalSpecialFists(Clone)") && rollCounter == 0)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/CrystalRoll", player.transform.position);
			enemies = Object.FindObjectsOfType<Enemy>();
			sendShrapnel(player.transform.position);
		}
		if (rollCounter < 2)
		{
			rollCounter++;
		}
		else
		{
			rollCounter = 0;
		}
	}

	private IEnumerator slamHands()
	{
		GameObject fistInstance = Object.Instantiate(crystalFists, playerGO.transform.position, Quaternion.identity);
		Movement movement = playerGO.GetComponent<Movement>();
		float duration = 4f;
		if (movement != null)
		{
			movement.AddSpeedEffect(-0.2f, duration, "CrystalSpecial");
		}
		CoroutineEmitter.Instance.StartCoroutine(followPlayer(fistInstance));
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/CrystalSlam", playerGO.transform.position);
		sendShrapnelLarge(playerGO.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
		yield return new WaitForSeconds(1.5f);
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/CrystalSlam", playerGO.transform.position);
		sendShrapnelLarge(playerGO.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
		yield return new WaitForSeconds(2.5f);
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/CrystalSlam", playerGO.transform.position);
		sendShrapnelLarge(playerGO.transform.position);
		Screenshake.Instance.AddTrauma(0.4f);
		if (movement != null)
		{
			movement.RemoveSpeedEffect("CrystalSpecial", removeAll: true);
		}
		isCoroutineRunning = false;
	}

	private IEnumerator followPlayer(GameObject fistInstance)
	{
		while (fistInstance != null && playerGO != null)
		{
			fistInstance.transform.position = playerGO.transform.position;
			yield return null;
		}
	}

	private void sendShrapnel(Vector3 position)
	{
		float num = 0f;
		for (int i = 0; i < rollShrapnel; i++)
		{
			Object.Instantiate(prefab, position, Quaternion.Euler(new Vector3(0f, 0f, num)));
			num += 90f;
		}
	}

	private void sendShrapnelLarge(Vector3 position)
	{
		float num = 0f;
		for (int i = 0; i < mainShrapnel * 2; i++)
		{
			Object.Instantiate(prefab, position, Quaternion.Euler(new Vector3(0f, 0f, num)));
			num += 360f / (float)(mainShrapnel * 2);
		}
	}
}
