using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunManager : MonoBehaviour
{
	public delegate void EnemyDeathCallback(GameObject attacker = null, GameObject enemy = null);

	public delegate void EnemyDamageCallback(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null);

	public static RunManager Instance;

	private List<Spell> allSpells;

	public List<Spell> availableSpells;

	private List<Relic> allRelics;

	public List<Relic> availableRelics;

	public GameObject healthPickupPrefab;

	public GameObject spellPickupPrefab;

	public GameObject relicPickupPrefab;

	[Header("UI")]
	public GameObject floorIntroUIWrapper;

	public TextMeshProUGUI floorNameText;

	public TextMeshProUGUI floorNumberText;

	private int enemiesKilled;

	public event EnemyDeathCallback OnKillEnemy;

	public event EnemyDamageCallback OnDamageEnemy;

	private void Awake()
	{
		Instance = this;
		allSpells = new List<Spell>(availableSpells);
		allRelics = new List<Relic>(availableRelics);
	}

	public void KilledEnemy(GameObject attacker = null, GameObject enemy = null)
	{
		enemiesKilled++;
		this.OnKillEnemy?.Invoke(attacker, enemy);
	}

	public void DamagedEnemy(float damage, bool causedDeath, GameObject beingDamaged, GameObject attacker = null)
	{
		this.OnDamageEnemy?.Invoke(damage, causedDeath, beingDamaged, attacker);
	}

	public void ShowFloorIntroText(string floorName, string floorNumber)
	{
		Debug.Log("Showing Floor Intro Text: " + floorName + " " + floorNumber);
		if (floorIntroUIWrapper != null)
		{
			floorNameText.text = floorName;
			floorNumberText.text = floorNumber;
			floorIntroUIWrapper.SetActive(value: true);
		}
	}

	public static Spell GetSpellByID(string id)
	{
		Spell spell = Instance.GetSpell(id);
		Instance.availableSpells.Remove(spell);
		return spell;
	}

	public static Spell GetSpell(bool forceMain = false, bool forceStartingSpell = false)
	{
		if (Instance.availableSpells.Count > 0)
		{
			Spell spell;
			if (forceMain)
			{
				List<Spell> list = new List<Spell>();
				foreach (Spell availableSpell in Instance.availableSpells)
				{
					if (!typeof(SpecialSpell).IsAssignableFrom(availableSpell.GetType()))
					{
						list.Add(availableSpell);
					}
				}
				if (list.Count == 0)
				{
					Debug.LogWarning("No more available main spells.");
					return null;
				}
				spell = list[Random.Range(0, list.Count)];
			}
			else if (forceStartingSpell)
			{
				List<Spell> list2 = new List<Spell>();
				foreach (Spell availableSpell2 in Instance.availableSpells)
				{
					if (availableSpell2.startingSpell)
					{
						list2.Add(availableSpell2);
					}
				}
				if (list2.Count == 0)
				{
					Debug.LogWarning("No more available starting spells.");
					return null;
				}
				spell = list2[Random.Range(0, list2.Count)];
			}
			else
			{
				spell = Instance.availableSpells[Random.Range(0, Instance.availableSpells.Count)];
			}
			Instance.availableSpells.Remove(spell);
			return Object.Instantiate(spell);
		}
		Debug.LogWarning("No more available spells.");
		return null;
	}

	public static Relic GetRelic()
	{
		List<Relic> list = RelicCollector.Instance.Relics();
		List<Relic> list2 = new List<Relic>();
		foreach (Relic availableRelic in Instance.availableRelics)
		{
			if (!list.Contains(availableRelic))
			{
				list2.Add(availableRelic);
			}
		}
		if (list2.Count > 0)
		{
			Relic relic = list2[Random.Range(0, list2.Count)];
			Instance.availableRelics.Remove(relic);
			return relic;
		}
		return null;
	}

	public static int GetAvailableRelicCount()
	{
		List<Relic> list = RelicCollector.Instance.Relics();
		int num = 0;
		foreach (Relic availableRelic in Instance.availableRelics)
		{
			if (!list.Contains(availableRelic))
			{
				num++;
			}
		}
		return num;
	}

	public GameObject SpawnRandomSpell(Vector3 position, bool forceMain = false)
	{
		Spell spell = GetSpell(forceMain);
		if (spell == null)
		{
			return null;
		}
		GameObject obj = Object.Instantiate(spellPickupPrefab, position, Quaternion.identity, MapGenerator.Instance.transform);
		spell = Object.Instantiate(spell);
		obj.GetComponent<SpellPickup>().spell = spell;
		return obj;
	}

	public Spell SpawnSpell(string id)
	{
		Spell spell = GetSpell(id);
		if (spell != null)
		{
			return SpawnSpell(spell);
		}
		return null;
	}

	public Spell SpawnSpell(Spell spell)
	{
		Transform parent = null;
		if (MapGenerator.Instance != null)
		{
			parent = MapGenerator.Instance.transform;
		}
		GameObject obj = Object.Instantiate(spellPickupPrefab, base.transform.position, Quaternion.identity, parent);
		spell = Object.Instantiate(spell);
		obj.GetComponent<SpellPickup>().spell = spell;
		return spell;
	}

	public Spell GetSpell(string id)
	{
		foreach (Spell allSpell in allSpells)
		{
			if (allSpell.GetType().ToString() == id)
			{
				return allSpell;
			}
		}
		return null;
	}

	public GameObject SpawnRandomRelic(Vector3 position)
	{
		return Object.Instantiate(relicPickupPrefab, position, Quaternion.identity, MapGenerator.Instance.transform);
	}

	public Relic SpawnRelic(string id)
	{
		foreach (Relic allRelic in allRelics)
		{
			if (allRelic.GetType().ToString() == id)
			{
				Object.Instantiate(relicPickupPrefab, base.transform.position, Quaternion.identity, MapGenerator.Instance.transform).GetComponent<RelicPickup>().relic = allRelic;
				return allRelic;
			}
		}
		return null;
	}

	public GameObject SpawnHealthPickup(Vector3 position)
	{
		Transform parent = null;
		if (MapGenerator.Instance != null)
		{
			parent = MapGenerator.Instance.transform;
		}
		return Object.Instantiate(healthPickupPrefab, position, Quaternion.identity, parent);
	}
}
