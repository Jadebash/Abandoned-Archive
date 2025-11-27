using System.Collections.Generic;
using UnityEngine;

public class TetheringLine : MonoBehaviour
{
	public Transform from;

	public Transform to;

	public bool lightning;

	public bool damageTo = true;

	public bool damageFrom;

	public bool usePathfinding;

	private List<Vector2> currentPath = new List<Vector2>();

	private float pathSearchTimer = 1f;

	private float pathfindingFailureCooldown;

	public float tickSpeed = 0.5f;

	public bool closeToDamage;

	public float maxLength = 5f;

	public float damagePerSecond = 10f;

	public bool aoeDamageBurst;

	public float aoeDamage = 2f;

	public bool moveLight;

	public bool rotateLight;

	public Transform lightTransform;

	public float fromMovementSpeedModifier;

	private LineRenderer lr;

	private float damageTimer;

	private float randomX;

	private float randomY;

	private void Start()
	{
		lr = GetComponent<LineRenderer>();
		randomX = Random.Range(0, 10000);
		randomY = Random.Range(0, 10000);
		Update();
		if (fromMovementSpeedModifier != 0f && (bool)from.GetComponent<Movement>())
		{
			from.GetComponent<Movement>().speedModifier += fromMovementSpeedModifier;
		}
	}

	private float DistanceMultiplier(float dist)
	{
		return Mathf.Clamp01(1f - dist / maxLength / 1.05f);
	}

	public Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
	{
		int num = 0;
		int num2 = 0;
		if (smoothness < 1f)
		{
			smoothness = 1f;
		}
		num = arrayToCurve.Length;
		num2 = num * Mathf.RoundToInt(smoothness) - 1;
		List<Vector3> list = new List<Vector3>(num2);
		float num3 = 0f;
		for (int i = 0; i < num2 + 1; i++)
		{
			num3 = Mathf.InverseLerp(0f, num2, i);
			List<Vector3> list2 = new List<Vector3>(arrayToCurve);
			for (int num4 = num - 1; num4 > 0; num4--)
			{
				for (int j = 0; j < num4; j++)
				{
					list2[j] = (1f - num3) * list2[j] + num3 * list2[j + 1];
				}
			}
			list.Add(list2[0]);
		}
		return list.ToArray();
	}

	private float CalculatePathLength(List<Vector2> path)
	{
		float num = 0f;
		if (path.Count > 0)
		{
			num += Vector2.Distance(to.position, path[0]);
			for (int i = 0; i < path.Count - 1; i++)
			{
				num += Vector2.Distance(path[i], path[i + 1]);
			}
			return num + Vector2.Distance(path[path.Count - 1], from.position);
		}
		return Vector2.Distance(to.position, from.position);
	}

	private void Update()
	{
		if (from == null || to == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		float num = Vector2.Distance(to.position, from.position);
		if (lightning)
		{
			Vector3[] array = new Vector3[5];
			array[0] = from.position;
			array[2] = (to.position + from.position) / 2f + new Vector3(Mathf.PerlinNoise(randomX, Time.time * 15f) - 0.5f, Mathf.PerlinNoise(randomY, Time.time * 15f) - 0.5f) * 0.5f;
			array[1] = (array[2] + from.position) / 2f + new Vector3(Mathf.PerlinNoise(randomX / 2f, Time.time * 15f) - 0.5f, Mathf.PerlinNoise(randomY / 2f, Time.time * 15f) - 0.5f) * 0.5f;
			array[3] = (to.position + array[2]) / 2f + new Vector3(Mathf.PerlinNoise(randomX * 2f, Time.time * 15f) - 0.5f, Mathf.PerlinNoise(randomY * 2f, Time.time * 15f) - 0.5f) * 0.5f;
			array[4] = to.position;
			lr.SetPositions(array);
		}
		else if (usePathfinding)
		{
			if (num > maxLength * 2f)
			{
				to = null;
				return;
			}
			if (pathfindingFailureCooldown > 0f)
			{
				pathfindingFailureCooldown -= Time.deltaTime;
			}
			else
			{
				pathSearchTimer += Time.deltaTime;
				if (pathSearchTimer >= 0.1f)
				{
					PathRequestManager.RequestPath(to.position, from.position, OnPathFound);
					pathSearchTimer = 0f;
				}
			}
			Vector3[] array2 = new Vector3[currentPath.Count + 1];
			array2[0] = to.position;
			for (int i = 0; i < currentPath.Count; i++)
			{
				array2[i + 1] = currentPath[i];
			}
			array2[^1] = from.position;
			Vector3[] array3 = MakeSmoothCurve(array2, 3f);
			lr.positionCount = array3.Length;
			lr.SetPositions(array3);
			num = CalculatePathLength(currentPath);
		}
		else
		{
			Vector3[] positions = new Vector3[2] { from.position, to.position };
			lr.SetPositions(positions);
		}
		if (closeToDamage)
		{
			lr.widthMultiplier = 0.15f * DistanceMultiplier(num);
			if (num > maxLength)
			{
				to = null;
				return;
			}
		}
		damageTimer -= Time.deltaTime;
		if (damageTimer <= 0f)
		{
			damageTimer += tickSpeed;
			float num2 = 1f;
			if (closeToDamage)
			{
				num2 = DistanceMultiplier(num);
			}
			if (damageTo && (bool)to.GetComponent<Health>())
			{
				to.GetComponent<Health>().Damage(Mathf.RoundToInt(damagePerSecond * tickSpeed * num2));
				if (aoeDamageBurst)
				{
					Enemy[] array4 = Object.FindObjectsOfType<Enemy>();
					foreach (Enemy enemy in array4)
					{
						if (Vector3.Distance(enemy.transform.position, to.position) < 3f && enemy.transform != to)
						{
							enemy.GetComponent<Health>().Damage(aoeDamage);
						}
					}
				}
			}
			if (damageFrom && (bool)from.GetComponent<Health>())
			{
				from.GetComponent<Health>().Damage(Mathf.RoundToInt(damagePerSecond * tickSpeed * num2));
			}
		}
		if (moveLight)
		{
			lightTransform.position = to.position;
			if (rotateLight)
			{
				lightTransform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, from.position - to.position, Vector3.forward), Vector3.forward);
			}
		}
	}

	public void OnPathFound(Vector2[] newPath, bool success)
	{
		if (!(to != null) || !(from != null))
		{
			return;
		}
		if (!success)
		{
			newPath = new Vector2[0];
			pathfindingFailureCooldown = 1f;
			return;
		}
		currentPath = new List<Vector2>();
		for (int i = 0; i < newPath.Length; i++)
		{
			currentPath.Add(newPath[i]);
		}
	}

	private void OnDestroy()
	{
		if (from != null && fromMovementSpeedModifier != 0f && (bool)from.GetComponent<Movement>())
		{
			from.GetComponent<Movement>().speedModifier -= fromMovementSpeedModifier;
		}
	}
}
