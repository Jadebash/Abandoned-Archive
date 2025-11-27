using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stance
{
	public enum StanceEffect
	{
		SpeedIncrease = 0,
		SpeedDecrease = 1,
		DamageIncrease = 2,
		DamageDecrease = 3
	}

	public bool enabled = true;

	public Color uiTint = Color.white;

	public List<SpellSlot> spellSlots = new List<SpellSlot>();

	public List<StanceEffect> effects = new List<StanceEffect>();
}
