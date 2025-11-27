using System.Collections;
using UnityEngine;

public class EnemyNail : Enemy
{
	public GameObject nails;

	public Animator anim;

	public GameObject arm;

	public SpriteRenderer armSprite;

	public Transform nailSpawn;

	private void Aggro()
	{
	}

	public override void Attack()
	{
		if (!GameObject.Find("Nails(Clone)"))
		{
			anim.ResetTrigger("ArmDown");
			spriteAnimator.SetBool("Attacking", value: true);
			spriteAnimator.SetBool("Walking", value: false);
			StartCoroutine(Shoot());
		}
	}

	public override bool CancelAttack(CancelAttackReason reason)
	{
		spriteAnimator.SetBool("Attacking", value: false);
		arm.SetActive(value: false);
		return true;
	}

	private IEnumerator Shoot()
	{
		if (spriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && spriteAnimator.GetBool("Attacking") && !base.transform.Find("Nails(Clone)"))
		{
			ChangeState(EnemyState.Idle);
			anim.SetTrigger("ArmDown");
			GameObject player = PlayerManager.ClosestPlayer(base.transform.position);
			arm.SetActive(value: true);
			yield return new WaitForSeconds(1f);
			GameObject obj = Object.Instantiate(nails, nailSpawn.position, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
			obj.transform.parent = base.transform;
			Vector3 vector = player.transform.position - base.transform.position;
			obj.transform.up = vector.normalized;
			yield return new WaitForSeconds(0.5f);
			arm.SetActive(value: false);
		}
	}
}
