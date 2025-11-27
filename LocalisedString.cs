using System;

[Serializable]
public struct LocalisedString
{
	public string key;

	public string value => LocalisationSystem.GetLocalisedValue(key);

	public LocalisedString(string key)
	{
		this.key = key;
	}

	public static implicit operator LocalisedString(string key)
	{
		return new LocalisedString(key);
	}
}
