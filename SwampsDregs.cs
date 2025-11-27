using System.Collections.Generic;
using UnityEngine;

public class SwampsDregs : MonoBehaviour
{
	public GameObject player;

	private List<GameObject> enemies = new List<GameObject>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Enemy" && !enemies.Contains(collision.gameObject))
		{
			enemies.Add(collision.gameObject);
			collision.gameObject.GetComponent<Health>().Poison(5);
		}
	}
}
