using UnityEngine;

public class AttackTelegraph : MonoBehaviour
{
	private LineRenderer lineRenderer;

	private float duration;

	private float timer;

	private Color startColor = new Color(1f, 0f, 0f, 0f);

	private Color endColor = new Color(1f, 0f, 0f, 0.8f);

	public bool preserveAfterDuration;

	private Transform trackedProjectile;

	private bool hasAttached;

	private Vector3 startPoint;

	private Vector3 endPoint;

	private float preserveTimer;

	private const float MAX_PRESERVE_TIME = 3f;

	public void Initialize(Vector3 start, Vector3 direction, float length, float duration, float width)
	{
		this.duration = duration;
		timer = 0f;
		startPoint = start;
		lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.startColor = startColor;
		lineRenderer.endColor = startColor;
		lineRenderer.startWidth = width;
		lineRenderer.endWidth = width;
		lineRenderer.numCapVertices = 5;
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition(0, start);
		int mask = LayerMask.GetMask("Ground", "Walls");
		RaycastHit2D raycastHit2D = Physics2D.Raycast(start, direction, length, mask);
		endPoint = start + direction.normalized * length;
		if (raycastHit2D.collider != null)
		{
			endPoint = raycastHit2D.point;
		}
		lineRenderer.SetPosition(1, endPoint);
		lineRenderer.sortingOrder = 10;
	}

	public void AttachProjectile(Transform projectile)
	{
		trackedProjectile = projectile;
		hasAttached = true;
		preserveAfterDuration = false;
	}

	private void Update()
	{
		if (trackedProjectile != null)
		{
			lineRenderer.SetPosition(0, trackedProjectile.position);
			Vector3 vector = endPoint - startPoint;
			if (Vector3.Dot(trackedProjectile.position - startPoint, vector.normalized) > vector.magnitude)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				UpdateGradient(endColor);
			}
			return;
		}
		if (hasAttached)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		timer += Time.deltaTime;
		float num = Mathf.Clamp01(timer / duration);
		if (num >= 1f)
		{
			if (preserveAfterDuration && !hasAttached)
			{
				preserveTimer += Time.deltaTime;
				if (preserveTimer >= 3f)
				{
					Object.Destroy(base.gameObject);
				}
				else
				{
					UpdateGradient(endColor);
				}
			}
			else if (!preserveAfterDuration && !hasAttached)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				UpdateGradient(endColor);
			}
		}
		else
		{
			Color baseColor = Color.Lerp(startColor, endColor, num);
			UpdateGradient(baseColor);
		}
	}

	private void UpdateGradient(Color baseColor)
	{
		Gradient gradient = new Gradient();
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(baseColor, 0f),
			new GradientColorKey(baseColor, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(baseColor.a, 0f),
			new GradientAlphaKey(baseColor.a, 0.7f),
			new GradientAlphaKey(0f, 1f)
		});
		lineRenderer.colorGradient = gradient;
	}
}
