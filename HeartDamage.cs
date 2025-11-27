using UnityEngine;

public class HeartDamage : MonoBehaviour
{
	private Animator anim;

	private GameObject player;

	public float threshold;

	private Health healthScript;

	private void Start()
	{
		anim = base.gameObject.GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player");
		healthScript = player.GetComponent<Health>();
	}

	private void Update()
	{
		anim.SetBool("tookDamage", healthScript.health <= threshold);
	}
}
