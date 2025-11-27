using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceLog Command", menuName = "Abandoned Archive/Debug/Commands/ForceLog Command")]
public class ForceLogCommand : ConsoleCommand
{
	private bool isPlaying;

	private EventInstance ccSound;

	public override CommandResponse Process(string[] args)
	{
		if (!isPlaying)
		{
			ccSound = RuntimeManager.CreateInstance("event:/Music/Other/Casino");
			ccSound.start();
			isPlaying = true;
		}
		else
		{
			ccSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			isPlaying = false;
		}
		return Success("tomyomy worm");
	}
}
