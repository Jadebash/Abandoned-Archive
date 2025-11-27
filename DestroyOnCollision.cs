using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
	private GameObject player;

	private Health[] enemies;

	private void Start()
	{
		enemies = Object.FindObjectsOfType<Health>();
		Health[] array = enemies;
		for (int i = 0; i < array.Length; i++)
		{
			Physics2D.IgnoreCollision(array[i].gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
		}
		player = PlayerManager.ClosestPlayer(base.transform.position);
	}

	private void OnCollisionEnter2D()
	{
		Object.Destroy((base.transform.Find("Circle")?.gameObject).gameObject);
	}
}
