using UnityEngine;

public class AdjustEnemyPower : MonoBehaviour
{
	private Health health;

	private void Start()
	{
		health = GetComponent<Health>();
	}
}
