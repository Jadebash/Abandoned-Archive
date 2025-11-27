using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Dialogue/Speaker")]
public class DialogueSpeaker : ScriptableObject
{
	public new LocalisedString name;

	public Sprite portrait;

	public EventReference sfx;
}
