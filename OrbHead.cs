using FMODUnity;
using UnityEngine;

public class OrbHead : Enemy
{
	public GameObject projectilePrefab;

	public GameObject orbSpawnPos;

	private bool flippedPos;

	private void Aggro()
	{
	}

	public override void Attack()
	{
		if (GameObject.Find("Orb(Clone)") == null)
		{
			spriteAnimator.SetBool("Attacking", value: true);
		}
	}

	public override bool CancelAttack(CancelAttackReason reason)
	{
		spriteAnimator.SetBool("Attacking", value: false);
		return true;
	}

	public void SpawnProjectile()
	{
		if (!(GetComponent<Health>().health <= 0f) && GameObject.Find("Orb(Clone)") == null)
		{
			Debug.Log("SpawnProjectile");
			if (base.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX && !flippedPos)
			{
				orbSpawnPos.transform.localPosition = new Vector3(orbSpawnPos.transform.localPosition.x * -1f, orbSpawnPos.transform.localPosition.y, orbSpawnPos.transform.localPosition.z);
				flippedPos = true;
			}
			else if (!base.transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX && flippedPos)
			{
				orbSpawnPos.transform.localPosition = new Vector3(orbSpawnPos.transform.localPosition.x * -1f, orbSpawnPos.transform.localPosition.y, orbSpawnPos.transform.localPosition.z);
				flippedPos = false;
			}
			Object.Instantiate(projectilePrefab, orbSpawnPos.transform.position, Quaternion.identity);
			Debug.Log("Spawned Projectile");
			ChangeState(EnemyState.Idle);
			spriteAnimator.SetTrigger("GrowingHead");
		}
	}

	private void GrowSoundEffect()
	{
		RuntimeManager.PlayOneShot("event:/SFX/Enemies/GrowHead", base.transform.position);
	}
}
