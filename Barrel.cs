using UnityEngine;

public class Barrel : MonoBehaviour
{
	public GameObject explosionPrefab;

	private void Start()
	{
		GetComponent<Health>().OnDeath += Destroy;
	}

	private void Destroy(GameObject attacker = null)
	{
		GetComponent<Health>().OnDeath -= Destroy;
		Screenshake.Instance.AddTrauma(0.6f);
		Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}
