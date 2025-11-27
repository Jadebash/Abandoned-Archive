using UnityEngine;

public abstract class SpecialSpell : Spell
{
	public LocalisedString rollName;

	public LocalisedString rollDescription;

	[HideInInspector]
	public bool isCoroutineRunning;

	[HideInInspector]
	public float cooldownTimer;

	public abstract void Roll(GameObject player);
}
