using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Time Main 2")]
public class TimeMain2 : Spell
{
	[HideInInspector]
	public float maxChargeTime = 2f;

	[HideInInspector]
	public float movementSpeedMultiplier = 1f;

	[HideInInspector]
	public float swordSizeMultiplier = 1f;

	[HideInInspector]
	public float stunTimeLocal;

	public GameObject smokePrefab;

	public GameObject timeLinePrefab;

	private EventInstance ChargeUpSound;

	[HideInInspector]
	public bool doDestroyProjectiles;

	private GameObject prefabInstance;

	private Vector3 originalLocation;

	private List<Vector3> positionList = new List<Vector3>();

	private GameObject player;

	private bool doesStun;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			damage *= 1.35f;
			break;
		case 2:
			movementSpeedMultiplier *= 1.5f;
			break;
		case 3:
			doDestroyProjectiles = true;
			break;
		case 4:
			stunTimeLocal += 0.2f;
			swordSizeMultiplier *= 0.85f;
			break;
		case 5:
			swordSizeMultiplier *= 1.25f;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		if (WaveManager.Instance != null)
		{
			WaveManager.Instance.OnEnteredRoom += CancelTeleport;
		}
		chargeTimer += Time.deltaTime;
		if (prefabInstance == null)
		{
			prefabInstance = Object.Instantiate(prefab, player.transform);
			prefabInstance.transform.GetChild(1).GetComponent<DamageOnEnter>().damage = Mathf.RoundToInt(damage * damageMultiplier);
			if (doesStun)
			{
				prefabInstance.transform.GetChild(1).GetComponent<DamageOnEnter>().doStun = true;
			}
			prefabInstance.transform.GetChild(1).GetComponent<DamageOnEnter>().stunTime = stunTimeLocal;
			if (doDestroyProjectiles)
			{
				prefabInstance.transform.GetChild(1).GetComponent<DestroyBulletOnCollision>().doDestroy = true;
			}
			prefabInstance.gameObject.transform.localScale *= swordSizeMultiplier;
			ChargeUpSound = RuntimeManager.CreateInstance("event:/SFX/Main Character/Spells/Time_Main2");
			ChargeUpSound.start();
			prefabInstance.transform.GetChild(2).GetComponent<DamageOverTime>().damagePerSecond = damage * damageMultiplier * 16f;
			originalLocation = player.transform.position;
			player.GetComponent<Movement>().enabled = false;
			player.GetComponent<Movement>().anim.SetBool("spin", value: true);
			this.player = player;
			Screenshake.Instance.AddTrauma(0.8f);
		}
		TrailRenderer[] componentsInChildren = prefabInstance.GetComponentsInChildren<TrailRenderer>();
		TrailRenderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		if (Vector3.Distance(player.transform.position, targetPoint) > 0.5f)
		{
			player.GetComponent<Rigidbody2D>().velocity = (targetPoint - player.transform.position).normalized * movementSpeedMultiplier * 4f;
		}
		array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		prefabInstance.transform.Rotate(0f, 0f, 1440f * Time.deltaTime);
		Screenshake.Instance.AddTrauma(2f * Time.deltaTime);
		prefabInstance.transform.localPosition = (targetPoint - player.transform.position).normalized * 0.1f;
		positionList.Add(player.transform.position);
		if (chargeTimer >= maxChargeTime || (chargeTimer >= 1f && releasedEarly))
		{
			Charged(damageMultiplier);
		}
	}

	public void CancelTeleport(DungeonRoom room)
	{
		originalLocation = player.transform.position;
	}

	public override void Charged(float damageMultiplier = 4f, bool clear = false)
	{
		if (chargeTimer < 1f && !clear)
		{
			releasedEarly = true;
			Execute(player, damageMultiplier);
			return;
		}
		base.Charged();
		if (WaveManager.Instance != null)
		{
			WaveManager.Instance.OnEnteredRoom -= CancelTeleport;
		}
		Object.Destroy(prefabInstance);
		Screenshake.Instance.AddTrauma(0.6f);
		if (positionList.Count >= 5)
		{
			CoroutineEmitter.Instance.StartCoroutine(DrawTimeLine(positionList));
		}
		positionList.Clear();
		ChargeUpSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		Object.Instantiate(smokePrefab, player.transform.position, Quaternion.identity);
		player.GetComponent<Movement>().SafeTeleport(originalLocation);
		Object.Instantiate(smokePrefab, player.transform.position, Quaternion.identity);
		player.GetComponent<Movement>().anim.SetBool("spin", value: false);
		player.GetComponent<Movement>().enabled = true;
		prefabInstance = null;
		Health component = player.GetComponent<Health>();
		component.invincibilityFrames = true;
		component.invincibilityTimer = 0f;
		releasedEarly = false;
	}

	private IEnumerator DrawTimeLine(List<Vector3> positions)
	{
		int num = 5;
		LineRenderer timeLine = Object.Instantiate(timeLinePrefab).GetComponent<LineRenderer>();
		float waitTime = 0.1f / (float)num;
		List<Vector3> drawnPositions = new List<Vector3>();
		positions.Reverse();
		List<List<Vector3>> list = SplitList(positions, Mathf.RoundToInt(positions.Count / num));
		List<Vector3>[] array = list.ToArray();
		foreach (List<Vector3> list2 in array)
		{
			if (timeLine == null)
			{
				break;
			}
			foreach (Vector3 item in list2)
			{
				drawnPositions.Add(item);
			}
			timeLine.positionCount = drawnPositions.Count;
			timeLine.SetPositions(drawnPositions.ToArray());
			yield return new WaitForSeconds(waitTime);
		}
	}

	public static List<List<Vector3>> SplitList(List<Vector3> positions, int nSize = 5)
	{
		List<List<Vector3>> list = new List<List<Vector3>>();
		for (int i = 0; i < positions.Count; i += nSize)
		{
			list.Add(positions.GetRange(i, Mathf.Min(nSize, positions.Count - i)));
		}
		return list;
	}
}
