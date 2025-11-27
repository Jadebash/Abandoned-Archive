using FMODUnity;
using UnityEngine;

public class SoundEffectOnCollision : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D col)
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/Bosses/PoisonHittingWall", base.transform.position);
	}
}
