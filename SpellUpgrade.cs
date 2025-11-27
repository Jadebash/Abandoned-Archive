using System;
using UnityEngine;

[Serializable]
public class SpellUpgrade
{
	public bool stackable = true;

	public LocalisedString description;

	[HideInInspector]
	public bool applied;
}
