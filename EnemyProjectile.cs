using FMODUnity;
using UnityEngine;

public class EnemyProjectile : Enemy
{
	[HideInInspector]
	public int rhythm;

	public GameObject projectilePrefab;

	public RangedArm rangedArm;

	[Header("Projectile Stats")]
	[Range(0f, 3f)]
	public float spreadAmount = 0.5f;

	[Range(1f, 10f)]
	public float projectileForceModifier = 5f;

	[Range(0f, 2f)]
	public float spawnInFrontAmount = 0.4f;

	[Range(0f, 100f)]
	public float rhythmResetChance = 50f;

	[Range(1f, 5f)]
	public float rhythmResetAmount = 1.5f;

	public override void Attack()
	{
		spriteAnimator.SetBool("Attacking", value: true);
	}

	public override bool CancelAttack(CancelAttackReason reason)
	{
		spriteAnimator.SetBool("Attacking", value: false);
		return true;
	}

	public void Shoot()
	{
		if (!(base.currentTarget == null))
		{
			Vector3 vector = (rangedArm.arm.flipX ? rangedArm.transform.right : (-rangedArm.transform.right));
			Vector3 vector2 = new Vector3(Random.insideUnitCircle.x - 0.5f, Random.insideUnitCircle.y - 0.5f) * spreadAmount * 2f + base.currentTarget.transform.position - base.transform.position;
			GameObject obj = Object.Instantiate(projectilePrefab, rangedArm.transform.position + vector.normalized * spawnInFrontAmount, Quaternion.identity);
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Shoot", base.transform.position);
			obj.GetComponent<Rigidbody2D>().AddForce(vector2.normalized * projectileForceModifier, ForceMode2D.Impulse);
		}
	}
}
