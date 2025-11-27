using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EnemyCharge : Enemy
{
	public GameObject chargeTrail;

	private Quaternion chargeDirection;

	[Header("Charge Stats")]
	[Range(0.5f, 10f)]
	public float chargeSpeedModifier = 8f;

	[Range(0f, 10f)]
	public float rotationSmoothing = 6f;

	[Range(5f, 60f)]
	public float damageAngle = 30f;

	[Range(0.5f, 3f)]
	public float maxDamageDistance = 1.2f;

	public float maxChargeTime = 5f;

	[Range(0f, 360f)]
	public float chargeRotationSpeed = 5f;

	[Range(0f, 1f)]
	public float wallHitStunTime = 0.7f;

	private float chargeTimer;

	private bool charging;

	private bool hasAttacked;

	public override void Init()
	{
		mainSoundEffectInstance = RuntimeManager.CreateInstance("event:/SFX/Enemies/MeleeAttack");
		mainSoundEffectInstance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, rb));
	}

	public override void Attack()
	{
		if (charging)
		{
			if (rb.velocity.magnitude < 0.2f && chargeTimer > 0.2f)
			{
				StopCharging();
			}
			rb.velocity = chargeDirection * Vector2.up * movementSpeed * chargeSpeedModifier;
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			chargeTimer += Time.deltaTime;
			if (chargeTimer > maxChargeTime)
			{
				StopCharging();
			}
			if (!hasAttacked && Vector2.Angle(rb.velocity.normalized, base.currentTarget.transform.position - base.transform.position) < damageAngle && Vector2.Distance(base.transform.position, base.currentTarget.transform.position) < maxDamageDistance)
			{
				DoAttackDamage();
				if ((bool)base.currentTarget.GetComponent<Rigidbody2D>())
				{
					base.currentTarget.GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.y, 0f - rb.velocity.x) * 5f * (maxDamageDistance / Vector2.Distance(base.transform.position, base.currentTarget.transform.position));
				}
				if ((bool)base.currentTarget.GetComponent<Movement>())
				{
					base.currentTarget.GetComponent<Movement>().Stun(0.3f);
				}
				hasAttacked = true;
			}
		}
		else if (attackTimer > timeBetweenAttacks)
		{
			float num = Vector2.Angle(Vector2.right, base.currentTarget.transform.position - base.transform.position);
			if (num > 90f)
			{
				num = 180f - num;
			}
			if (num < 45f)
			{
				attackTimer = 0f;
				spriteAnimator?.SetTrigger("StartTelegraphCharge");
			}
			else
			{
				pathOffset = (((base.currentTarget.transform.position - base.transform.position).x >= 0f) ? new Vector2(-3f, 0f) : new Vector2(3f, 0f));
				Navigate();
				mainSoundEffectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}
	}

	public override bool CancelAttack(CancelAttackReason reason)
	{
		if (reason == CancelAttackReason.TargetTooFar)
		{
			if (charging)
			{
				return false;
			}
			if (attackTimer < 3f)
			{
				return false;
			}
		}
		if (reason == CancelAttackReason.NoTargetRaycast && charging)
		{
			return false;
		}
		StopCharging();
		return true;
	}

	public override void OnHitWall(float speed)
	{
		if (speed > 2f && charging)
		{
			StopCharging();
			Stun(wallHitStunTime);
		}
	}

	private void StopCharging()
	{
		attackTimer = 0f;
		charging = false;
		if (rb != null && rb.velocity.magnitude < 1f)
		{
			rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
		}
		spriteAnimator?.ResetTrigger("StartTelegraphCharge");
		spriteAnimator?.SetTrigger("CancelCharge");
		chargeTrail?.SetActive(value: false);
		mainSoundEffectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void ChargeTelegraphSound()
	{
		mainSoundEffectInstance.start();
		mainSoundEffectInstance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, rb));
	}

	public void Charge()
	{
		if (base.State == EnemyState.Attacking)
		{
			charging = true;
			hasAttacked = false;
			chargeTimer = 0f;
		}
	}

	public override void Target()
	{
		Vector3 vector = base.currentTarget.transform.position - base.transform.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		if (!charging)
		{
			chargeDirection = Quaternion.Slerp(chargeDirection, Quaternion.AngleAxis(num - 90f, Vector3.forward), Time.deltaTime * rotationSmoothing);
			return;
		}
		if (Vector2.Angle(chargeDirection * Vector2.up, base.currentTarget.transform.position - base.transform.position) < damageAngle)
		{
			chargeDirection = Quaternion.RotateTowards(chargeDirection, Quaternion.AngleAxis(num - 90f, Vector3.forward), Time.deltaTime * chargeRotationSpeed);
		}
		if ((double)rb.velocity.magnitude > 0.5)
		{
			chargeTrail.SetActive(value: true);
		}
	}
}
