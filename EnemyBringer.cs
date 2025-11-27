using UnityEngine;

public class EnemyBringer : Enemy
{
	public GameObject spellPrefab;

	private Collider2D[] playableSpaceBounds;

	public override void Init()
	{
		base.Init();
		FindPlayableSpaceBounds();
	}

	private void FindPlayableSpaceBounds()
	{
		Collider2D[] array = Physics2D.OverlapPointAll(base.transform.position);
		foreach (Collider2D collider2D in array)
		{
			if (collider2D.CompareTag("PlayableSpace"))
			{
				playableSpaceBounds = collider2D.gameObject.GetComponents<Collider2D>();
				break;
			}
		}
	}

	private Vector3 ClampPositionToBounds(Vector3 position)
	{
		if (playableSpaceBounds == null || playableSpaceBounds.Length == 0)
		{
			return position;
		}
		Collider2D[] array = playableSpaceBounds;
		foreach (Collider2D collider2D in array)
		{
			if (collider2D != null && collider2D.OverlapPoint(position))
			{
				return position;
			}
		}
		Collider2D collider2D2 = null;
		float num = float.MaxValue;
		array = playableSpaceBounds;
		foreach (Collider2D collider2D3 in array)
		{
			if (!(collider2D3 == null))
			{
				Vector3 b = collider2D3.bounds.ClosestPoint(position);
				float num2 = Vector3.Distance(position, b);
				if (num2 < num)
				{
					num = num2;
					collider2D2 = collider2D3;
				}
			}
		}
		if (collider2D2 != null)
		{
			Bounds bounds = collider2D2.bounds;
			return new Vector3(Mathf.Clamp(position.x, bounds.min.x, bounds.max.x), Mathf.Clamp(position.y, bounds.min.y, bounds.max.y), position.z);
		}
		return position;
	}

	public override void Attack()
	{
		if (attackTimer > timeBetweenAttacks)
		{
			if (Vector3.Distance(base.currentTarget.transform.position, base.transform.position) <= 1.5f)
			{
				spriteAnimator.SetTrigger("Attack");
				spriteAnimator.ResetTrigger("Cast");
			}
			else
			{
				spriteAnimator.SetTrigger("Cast");
				spriteAnimator.ResetTrigger("Attack");
			}
		}
		else if (Vector3.Distance(base.currentTarget.transform.position, base.transform.position) <= 2f)
		{
			pathOffset = (((base.currentTarget.transform.position - base.transform.position).x >= 0f) ? new Vector2(-1.4f, 0f) : new Vector2(1.4f, 0f));
			Navigate();
		}
	}

	public override bool CancelAttack(CancelAttackReason reason)
	{
		switch (reason)
		{
		case CancelAttackReason.NoTargetRaycast:
			Cancel();
			return true;
		case CancelAttackReason.TargetTooFar:
			return false;
		case CancelAttackReason.Stunned:
			Cancel();
			return true;
		case CancelAttackReason.Fleeing:
			return true;
		default:
			Cancel();
			return true;
		}
	}

	private void Cancel()
	{
		attackTimer = 0f;
		spriteAnimator.ResetTrigger("Cast");
		spriteAnimator.ResetTrigger("Attack");
		spriteAnimator.SetTrigger("Cancel");
	}

	public void SpawnSpell()
	{
		Vector3 position = base.currentTarget.transform.position - new Vector3(0f, 0.5f, 0f);
		Rigidbody2D component = base.currentTarget.GetComponent<Rigidbody2D>();
		if (component != null)
		{
			position += (Vector3)component.velocity.normalized * 2.1f;
		}
		position = ClampPositionToBounds(position);
		Object.Instantiate(spellPrefab, position, Quaternion.identity);
		attackTimer = 0f;
	}

	public void Melee()
	{
		if (Vector3.Distance(base.currentTarget.transform.position, base.transform.position) <= 1.5f)
		{
			DoAttackDamage();
		}
		attackTimer = 0f;
	}
}
