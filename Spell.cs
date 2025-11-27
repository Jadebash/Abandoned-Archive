using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : ScriptableObject
{
	public new LocalisedString name;

	public School school;

	public Sprite icon;

	public LocalisedString flavour;

	public LocalisedString description;

	public LocalisedString lore;

	public float cooldownTime;

	public float damage;

	public TargetType target;

	[Tooltip("Perform special function if performed 3 times in quick succession")]
	public bool combo;

	[HideInInspector]
	public int comboCounter;

	public bool charge;

	[HideInInspector]
	public float chargeTimer;

	public bool canCastWhileRolling = true;

	public bool startingSpell;

	public GameObject prefab;

	public List<SpellUpgrade> upgrades = new List<SpellUpgrade>();

	[HideInInspector]
	public int upgradeCount;

	[HideInInspector]
	public bool releasedEarly;

	public abstract void Execute(GameObject player, float damageModifier = 1f, bool performCombo = false, GameObject target = null, Vector3 targetPoint = default(Vector3));

	public virtual void Charged(float damageMultiplier = 1f, bool clear = false)
	{
		chargeTimer = 0f;
	}

	public abstract void ApplyUpgrade(int upgrade);

	public void Upgrade(SpellUpgrade chosenUpgrade)
	{
		if (!ToString().Contains("(Clone)"))
		{
			Debug.LogError("Tried to modify the original scriptable object!");
		}
		else if (upgrades.Contains(chosenUpgrade))
		{
			upgradeCount++;
			chosenUpgrade.applied = true;
			ApplyUpgrade(upgrades.IndexOf(chosenUpgrade) + 1);
		}
		else
		{
			Debug.LogWarning("Upgrade out of range/not available. Upgrade Count Available: " + upgrades.Count);
		}
	}

	protected void CastAnimationOneHanded(GameObject player)
	{
		player.GetComponent<Movement>().anim.SetTrigger("castOneHand");
	}
}
