using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

public class OrbOfThunder : MonoBehaviour
{
	public float shockRange = 3f;

	public int shockDamage = 4;

	public GameObject prefab;

	public float damageMultiplier;

	public float shockDamageModifier;

	public bool canShockPlayer;

	private bool hasShockedPlayer;

	public float shockFreq = 0.5f;

	public float explosionDamageMoidifer;

	private List<GameObject> attackList = new List<GameObject>();

	private float timer;

	[HideInInspector]
	public GameObject player;

	private void Start()
	{
		InvokeRepeating("grabAttackList", 0f, shockFreq);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (!hasShockedPlayer && timer > 1f && canShockPlayer)
		{
			GameObject obj = Object.Instantiate(prefab);
			obj.GetComponent<TetheringLine>().damagePerSecond = 0f;
			obj.GetComponent<TetheringLine>().from = base.transform;
			obj.GetComponent<TetheringLine>().to = player.transform;
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Lightning_Main", player.transform.position);
			CoroutineEmitter.Instance.StartCoroutine(playerBoost());
			hasShockedPlayer = true;
			Screenshake.Instance.AddTrauma(0.5f);
		}
		if (attackList == null)
		{
			return;
		}
		if (attackList.Count > 0)
		{
			Screenshake.Instance.AddTrauma(0.5f);
		}
		GameObject[] array = attackList.ToArray();
		foreach (GameObject gameObject in array)
		{
			attackList.Remove(gameObject);
			if (!(gameObject == null))
			{
				GameObject obj2 = Object.Instantiate(prefab);
				obj2.GetComponent<TetheringLine>().from = base.transform;
				obj2.GetComponent<TetheringLine>().to = gameObject.transform;
				obj2.GetComponent<TetheringLine>().damagePerSecond = Mathf.Round((float)shockDamage * damageMultiplier * shockDamageModifier);
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Lightning_Main", gameObject.transform.position);
			}
		}
	}

	private List<GameObject> grabAttackList()
	{
		List<GameObject> list = GameObject.FindGameObjectsWithTag("Enemy").ToList();
		List<GameObject> list2 = new List<GameObject>();
		GameObject[] array = list.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(base.transform.position, gameObject.transform.position) < shockRange)
			{
				list2.Add(gameObject);
			}
		}
		attackList = list2;
		return list2;
	}

	private IEnumerator playerBoost()
	{
		player.GetComponent<Movement>().AddSpeedEffect(0.4f, 2f, "OrbOfThunder");
		player.GetComponent<SpellCasting>().spellDamageMultiplier += 0.2f;
		yield return new WaitForSeconds(2f);
		player.GetComponent<SpellCasting>().spellDamageMultiplier -= 0.2f;
	}
}
