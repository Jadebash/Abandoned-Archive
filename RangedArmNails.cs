using UnityEngine;

public class RangedArmNails : MonoBehaviour
{
	public EnemyNail enemy;

	[HideInInspector]
	public SpriteRenderer arm;

	private bool lastFlip;

	public float rotationSmooth;

	public Transform nailSpawn;

	private void Awake()
	{
		arm = GetComponentInChildren<SpriteRenderer>();
	}

	private void Update()
	{
		if (arm.flipX)
		{
			base.transform.localPosition = new Vector3(0.056f, 0.17f, 0f);
			arm.transform.localPosition = new Vector3(0.171f, 0f, 0f);
			nailSpawn.localPosition = new Vector3(0.05f, 0f, 0f);
		}
		else
		{
			base.transform.localPosition = new Vector3(-0.056f, 0.17f, 0f);
			arm.transform.localPosition = new Vector3(-0.171f, 0f, 0f);
			nailSpawn.localPosition = new Vector3(-0.05f, 0f, 0f);
		}
		if (enemy.currentTarget != null)
		{
			Vector3 vector = enemy.currentTarget.transform.position - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (!arm.flipX)
			{
				num -= 180f;
			}
			if (lastFlip != arm.flipX)
			{
				base.transform.localRotation = Quaternion.Euler(base.transform.rotation.x, base.transform.rotation.y, 0f - base.transform.rotation.z);
			}
			base.transform.localRotation = Quaternion.Slerp(base.transform.rotation, Quaternion.AngleAxis(num, Vector3.forward), Time.deltaTime * rotationSmooth);
			lastFlip = arm.flipX;
		}
	}

	private void OnDisable()
	{
		base.transform.localRotation = Quaternion.identity;
	}
}
