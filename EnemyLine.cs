using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EnemyLine : Enemy
{
	public Transform linePivot;

	public GameObject telegraphLine;

	public GameObject attackLine;

	private Vector3 targetPosition;

	[Header("Line Stats")]
	[Range(-180f, 0f)]
	public float telegraphStartRotation = -65f;

	[Range(50f, 300f)]
	public float sweepAngle = 150f;

	[Range(0f, 4f)]
	public float rotationSmooth = 2f;

	[Range(0f, 10f)]
	public float attackTime = 2f;

	public Animator lineAnimator;

	private bool attacking;

	public override void Init()
	{
		attackLine.SetActive(value: false);
		attackTimer = timeBetweenAttacks / attackTime;
		mainSoundEffectInstance = RuntimeManager.CreateInstance("event:/SFX/Enemies/Laser");
		RuntimeManager.AttachInstanceToGameObject(mainSoundEffectInstance, base.transform, rb);
	}

	public override void Attack()
	{
		if (attackTimer > timeBetweenAttacks && attackTimer < timeBetweenAttacks + telegraphTime)
		{
			targetPosition = base.currentTarget.transform.position;
			telegraphLine.SetActive(value: true);
			Target();
			attackLine.SetActive(value: false);
		}
		else if (attackTimer > timeBetweenAttacks + telegraphTime && !attacking)
		{
			mainSoundEffectInstance.start();
			targetPosition = base.currentTarget.transform.position;
			attacking = true;
		}
		else if (attackTimer > timeBetweenAttacks + telegraphTime && attackTimer < timeBetweenAttacks + telegraphTime + attackTime)
		{
			telegraphLine.SetActive(value: false);
			attackLine.SetActive(value: true);
		}
		else if (attackTimer > timeBetweenAttacks + telegraphTime + attackTime)
		{
			telegraphLine.SetActive(value: false);
			if (attackLine.activeSelf)
			{
				lineAnimator.SetTrigger("Stop");
			}
			attackTimer = 0f;
			attacking = false;
		}
	}

	public override bool CancelAttack(CancelAttackReason reason)
	{
		mainSoundEffectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		attackTimer = 0f;
		attacking = false;
		telegraphLine.SetActive(value: false);
		if (attackLine.activeSelf)
		{
			lineAnimator.SetTrigger("Stop");
		}
		return true;
	}

	public override void Target()
	{
		if (telegraphLine.activeSelf || attackLine.activeSelf)
		{
			Vector3 vector = targetPosition - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			float num2 = telegraphStartRotation;
			if (attackTimer > timeBetweenAttacks + telegraphTime)
			{
				num2 = telegraphStartRotation + (attackTimer - (timeBetweenAttacks + telegraphTime)) / attackTime * sweepAngle;
			}
			num -= num2;
			if (attackTimer > timeBetweenAttacks && attackTimer < timeBetweenAttacks + telegraphTime)
			{
				linePivot.rotation = Quaternion.AngleAxis(num, Vector3.forward);
			}
			else
			{
				linePivot.rotation = Quaternion.Slerp(linePivot.rotation, Quaternion.AngleAxis(num, Vector3.forward), Time.deltaTime * rotationSmooth);
			}
		}
	}

	public override void Stunned()
	{
		CancelAttack(CancelAttackReason.Stunned);
	}
}
