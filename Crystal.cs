using UnityEngine;

public class Crystal : MonoBehaviour
{
	public bool tutorial = true;

	public bool tutorialAnarchy = true;

	public int knowledgeDrop = 4;

	public GameObject explosionPrefab;

	public GameObject knowledgePelletPrefab;

	private void Start()
	{
		GetComponent<Health>().OnDeath += Destroy;
	}

	private void Destroy(GameObject attacker = null)
	{
		if (tutorial)
		{
			Object.FindObjectOfType<Tutorial>()?.DestroyedCrystal();
		}
		if (tutorialAnarchy)
		{
			Object.FindObjectOfType<AnarchyManager>()?.DestroyedCrystal();
		}
		Screenshake.Instance.AddTrauma(0.6f);
		Object.Instantiate(explosionPrefab, base.transform.position, Quaternion.identity);
		for (int i = 0; i < knowledgeDrop; i++)
		{
			Object.Instantiate(knowledgePelletPrefab, base.transform.position + new Vector3(Random.insideUnitCircle.x * 0.5f, Random.insideUnitCircle.y * 0.5f), Quaternion.identity);
		}
		Object.Destroy(base.gameObject);
	}
}
