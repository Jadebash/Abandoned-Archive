using FMODUnity;
using UnityEngine;

public class PropBreak : MonoBehaviour
{
	private Animator animator;

	private bool broken;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void Break()
	{
		if (!broken)
		{
			broken = true;
			RuntimeManager.PlayOneShot("event:/SFX/World/Drop", base.transform.position);
			animator.SetTrigger("Break");
		}
	}
}
