using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EnemyMelee : Enemy
{
	private Animator anim;

	public Transform attackPivot;

	[Header("Melee Stats")]
	[Range(0.5f, 5f)]
	public float attackingMovementDecrease = 3f;

	[Range(0f, 10f)]
	public float rotationSmoothing = 6f;

	public override void Init()
	{
		anim = GetComponent<Animator>();
		mainSoundEffectInstance = RuntimeManager.CreateInstance("event:/SFX/Enemies/MeleeAttack");
		mainSoundEffectInstance.set3DAttributes(base.gameObject.To3DAttributes());
	}

	public override void Attack()
	{
		Vector3 vector = base.currentTarget.transform.position - base.transform.position;
		rb.velocity = new Vector2(vector.normalized.x, vector.normalized.y) * (movementSpeed / attackingMovementDecrease);
		anim.SetBool("inRange", value: true);
		if (attackTimer > timeBetweenAttacks)
		{
			attackTimer = 0f;
			anim.SetTrigger("Attack");
			mainSoundEffectInstance = RuntimeManager.CreateInstance("event:/SFX/Enemies/MeleeAttack");
			mainSoundEffectInstance.set3DAttributes(base.gameObject.To3DAttributes());
			mainSoundEffectInstance.start();
		}
	}

	public override void Target()
	{
		Vector3 vector = base.currentTarget.transform.position - base.transform.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		attackPivot.rotation = Quaternion.Slerp(attackPivot.rotation, Quaternion.AngleAxis(num - 90f, Vector3.forward), Time.deltaTime * rotationSmoothing);
	}

	public override void Tick()
	{
		if (base.State != EnemyState.Attacking)
		{
			anim.SetBool("inRange", value: false);
			mainSoundEffectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}
}
