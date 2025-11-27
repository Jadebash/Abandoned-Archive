using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Spells/Earth Special")]
public class EarthSpecial : SpecialSpell
{
	[HideInInspector]
	public float scale = 1f;

	[HideInInspector]
	public float movementSpeedBuff = 0.3f;

	[HideInInspector]
	public bool chanceToHeal;

	[HideInInspector]
	public float healChance;

	private List<Health> targets;

	public GameObject vines;

	public GameObject diagonalVines;

	[HideInInspector]
	public float damageScale = 1f;

	[HideInInspector]
	public float distanceScale = 1f;

	[HideInInspector]
	public float stunScale = 1f;

	[HideInInspector]
	public bool rollDiagonal;

	public GameObject rock;

	private bool vinesPlaceable;

	public override void ApplyUpgrade(int upgrade)
	{
		switch (upgrade)
		{
		case 1:
			damageScale *= 1.3f;
			distanceScale *= 0.9f;
			break;
		case 2:
			cooldownTime *= 0.8f;
			stunScale *= 0.75f;
			break;
		case 3:
			distanceScale *= 1.25f;
			break;
		case 4:
			stunScale *= 1.25f;
			break;
		case 5:
			rollDiagonal = true;
			break;
		default:
			Debug.LogWarning("Upgrade out of bounds.");
			break;
		}
	}

	public override void Execute(GameObject player, float damageMultiplier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3))
	{
		targets = new List<Health>();
		Enemy[] array = Object.FindObjectsOfType<Enemy>();
		foreach (Enemy enemy in array)
		{
			if (Vector2.Distance(enemy.gameObject.transform.position, player.transform.position) <= 5f * distanceScale)
			{
				targets.Add(enemy.gameObject.GetComponent<Health>());
			}
		}
		Boss[] array2 = Object.FindObjectsOfType<Boss>();
		foreach (Boss boss in array2)
		{
			if (Vector2.Distance(boss.gameObject.transform.position, player.transform.position) <= 5f * distanceScale)
			{
				targets.Add(boss.gameObject.GetComponent<Health>());
			}
		}
		if (targets.Count > 0)
		{
			CoroutineEmitter.Instance.StartCoroutine(VineLockdown(targets, damageMultiplier, player));
			return;
		}
		player.GetComponent<SpellCasting>().SetSpellCooldownTime(this, cooldownTime);
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spell_Fail");
	}

	public override void Roll(GameObject player)
	{
		Movement component = player.GetComponent<Movement>();
		EarthSpecialVines[] array = Object.FindObjectsOfType<EarthSpecialVines>();
		int num = 0;
		EarthSpecialVines[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			_ = array2[i];
			num++;
		}
		if (num < 3)
		{
			vinesPlaceable = true;
		}
		else
		{
			vinesPlaceable = false;
		}
		if (component.targetVelocity.x != 0f && Mathf.Abs(component.targetVelocity.y) <= 0.4f && vinesPlaceable)
		{
			Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
		}
		if (!rollDiagonal || component.targetVelocity.x == 1f || component.targetVelocity.x == 0f || component.targetVelocity.x == -1f || !vinesPlaceable)
		{
			return;
		}
		if (component.targetVelocity.x > 0f)
		{
			if (component.targetVelocity.y > 0.4f)
			{
				Object.Instantiate(diagonalVines, player.transform.position, Quaternion.identity);
			}
			else if (component.targetVelocity.y < -0.4f)
			{
				Object.Instantiate(diagonalVines, player.transform.position, Quaternion.identity).transform.localScale = new Vector3(-1f, 1f, 1f);
			}
		}
		else if (component.targetVelocity.y > 0.4f)
		{
			Object.Instantiate(diagonalVines, player.transform.position, Quaternion.identity).transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else if (component.targetVelocity.y < -0.4f)
		{
			Object.Instantiate(diagonalVines, player.transform.position, Quaternion.identity);
		}
	}

	private IEnumerator VineLockdown(List<Health> targets, float damageMultiplier, GameObject player)
	{
		Dictionary<Health, Vector3> targetPositions = new Dictionary<Health, Vector3>();
		foreach (Health target in targets)
		{
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Vines_Growing", target.gameObject.transform.position);
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/EarthRockWoosh", target.gameObject.transform.position);
			Object.Instantiate(vines, target.gameObject.transform.position, Quaternion.identity);
			Enemy component = target.gameObject.GetComponent<Enemy>();
			if (component != null)
			{
				component.Stun(4f * stunScale, doStunStars: false);
			}
			if (target.gameObject.GetComponent<Boss>() == null)
			{
				target.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
				target.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
			}
			targetPositions.Add(target, target.gameObject.transform.position);
			Object.Instantiate(rock, target.gameObject.transform.position, Quaternion.identity);
		}
		yield return new WaitForSeconds(3f);
		foreach (Health target2 in targets)
		{
			if (!(target2 == null))
			{
				Vector3 vector = targetPositions[target2];
				if (target2.gameObject.GetComponent<Boss>() == null)
				{
					target2.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
				}
				RuntimeManager.PlayOneShot("event:/SFX/Main Character/Spells/Earth_Special_RockFall", target2.gameObject.transform.position);
				Screenshake.Instance.AddTrauma(0.4f);
				if (Vector2.Distance(vector, target2.transform.position) < 1f)
				{
					target2.gameObject.GetComponent<Health>().Damage(damage * damageMultiplier * damageScale);
				}
			}
		}
	}
}
