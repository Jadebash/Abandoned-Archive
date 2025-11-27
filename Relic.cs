using UnityEngine;

public abstract class Relic : ScriptableObject
{
	public new LocalisedString name;

	public Sprite icon;

	public LocalisedString loreDescription;

	public LocalisedString effect;

	[Tooltip("If true, this relic will be removed when transitioning to a new floor")]
	public bool boundToFloor;

	private RelicCollector collector;

	public abstract void GainedRelic(GameObject player);

	public abstract void LostRelic(GameObject player);

	public void SetCollector(RelicCollector newCollector)
	{
		collector = newCollector;
	}

	public void Use()
	{
		collector?.AnimateRelic(this);
	}
}
