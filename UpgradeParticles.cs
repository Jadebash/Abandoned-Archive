using UnityEngine;

public class UpgradeParticles : MonoBehaviour
{
	public Transform player;

	private float timer;

	private void Update()
	{
		timer += Time.deltaTime;
		if (player != null)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, player.transform.position, Time.deltaTime * Mathf.Pow(timer, 4f));
		}
		if (timer < 2f)
		{
			Screenshake.Instance.AddTrauma(timer * Time.deltaTime * 2f);
		}
	}
}
