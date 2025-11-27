using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
	private AudioSource audioSource;

	public List<AudioClip> footsteps;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void Step()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Main Character/Footsteps", base.transform.position);
	}
}
