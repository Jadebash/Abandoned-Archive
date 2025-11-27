using UnityEngine;

public class AttackHelper : MonoBehaviour
{
	private Collider2D[] playableSpaceBounds;

	private void Start()
	{
		FindPlayableSpaceBounds();
	}

	public void FindPlayableSpaceBounds(Vector3? positionToCheck = null)
	{
		Collider2D[] array = Physics2D.OverlapPointAll(positionToCheck.HasValue ? positionToCheck.Value : base.transform.position);
		bool flag = false;
		Collider2D[] array2 = array;
		foreach (Collider2D collider2D in array2)
		{
			if (collider2D.CompareTag("PlayableSpace"))
			{
				playableSpaceBounds = collider2D.gameObject.GetComponents<Collider2D>();
				flag = true;
				break;
			}
		}
		if (!flag && (playableSpaceBounds == null || playableSpaceBounds.Length == 0))
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("PlayableSpace");
			if (gameObject != null)
			{
				playableSpaceBounds = gameObject.GetComponents<Collider2D>();
			}
		}
	}

	public Vector3 ClampPositionToBounds(Vector3 position)
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
				Vector2 b = collider2D3.ClosestPoint(position);
				float num2 = Vector2.Distance(position, b);
				if (num2 < num)
				{
					num = num2;
					collider2D2 = collider2D3;
				}
			}
		}
		if (collider2D2 != null)
		{
			Vector2 vector = collider2D2.ClosestPoint(position);
			return new Vector3(vector.x, vector.y, position.z);
		}
		return position;
	}

	public Vector3 predictedPlayerPosition(GameObject player, float distanceMultiplier, bool isClamped = true, Vector3 offset = default(Vector3))
	{
		Vector3 vector = player.transform.position + offset;
		if (player.GetComponent<Rigidbody2D>() != null)
		{
			vector += (Vector3)player.GetComponent<Rigidbody2D>().velocity.normalized * distanceMultiplier;
		}
		if (isClamped)
		{
			return ClampPositionToBounds(vector);
		}
		return vector;
	}
}
