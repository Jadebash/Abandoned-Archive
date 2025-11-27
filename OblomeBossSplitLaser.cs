using System;
using UnityEngine;

public class OblomeBossSplitLaser : StateMachineBehaviour
{
	private GameObject duplicateLaser;

	private GameObject mainLaser;

	private GameObject player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player = PlayerManager.ClosestPlayer(animator.gameObject.transform.position);
		OblomeBoss component = animator.GetComponent<OblomeBoss>();
		if (component != null)
		{
			component.SpawnEnemy();
			if (component.GetComponent<Health>().health <= component.GetComponent<Health>().maxHealth / 2f)
			{
				component.SpawnEnemy();
			}
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (duplicateLaser == null)
		{
			duplicateLaser = GameObject.Find("DuplicateLaserObject");
		}
		if (mainLaser == null)
		{
			mainLaser = GameObject.Find("MainLaserObject");
		}
		Vector3 vector = player.transform.position - duplicateLaser.transform.position;
		float f = Vector3.SignedAngle(duplicateLaser.transform.up, vector, Vector3.forward);
		duplicateLaser.transform.up = Vector3.RotateTowards(duplicateLaser.transform.up, vector, MathF.PI / 180f * Mathf.Abs(f) * Time.deltaTime * 4f, 0f);
		Vector3 vector2 = player.transform.position - mainLaser.transform.position;
		float f2 = Vector3.SignedAngle(mainLaser.transform.up, vector2, Vector3.forward);
		mainLaser.transform.up = Vector3.RotateTowards(mainLaser.transform.up, vector2, MathF.PI / 180f * Mathf.Abs(f2) * Time.deltaTime * 2f, 0f);
	}
}
