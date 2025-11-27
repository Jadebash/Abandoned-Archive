using UnityEngine;

public class Rose : MonoBehaviour
{
	private Enemy enemyVar;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (!(collision.tag == "Player"))
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(gameObject.transform.position, collision.transform.position) < 3f)
			{
				enemyVar = gameObject.GetComponent<Enemy>();
				if (enemyVar != null)
				{
					enemyVar.Stun(1.5f);
				}
			}
		}
		Object.Destroy(base.gameObject);
	}
}
