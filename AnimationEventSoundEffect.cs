using FMODUnity;
using UnityEngine;

public class AnimationEventSoundEffect : MonoBehaviour
{
	public EventReference sfx;

	public void AnimationSoundEffect()
	{
		RuntimeManager.PlayOneShotAttached(sfx, base.gameObject);
	}

	public void RevealSoundEffect()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/CardReveal", base.transform.position);
	}
}
