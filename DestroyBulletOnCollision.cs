using FMODUnity;
using UnityEngine;

public class DestroyBulletOnCollision : MonoBehaviour
{
	public GameObject bulletDestroyPrefab;

	public bool doDestroy;

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Bullet" && doDestroy)
		{
			if (collision.gameObject.GetComponent<Projectile>() != null)
			{
				collision.gameObject.GetComponent<Projectile>().enabled = false;
			}
			Object.Destroy(collision.gameObject);
			if (bulletDestroyPrefab != null)
			{
				Object.Instantiate(bulletDestroyPrefab, collision.gameObject.transform.position, Quaternion.identity);
			}
			RuntimeManager.PlayOneShot("event:/SFX/Enemies/Projectile_Hit", collision.gameObject.transform.position);
		}
	}
}
