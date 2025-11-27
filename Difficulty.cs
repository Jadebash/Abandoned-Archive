using UnityEngine;

public class Difficulty : MonoBehaviour
{
	public static Difficulty Instance;

	[Header("Enemy Damage")]
	[Range(1f, 5f)]
	public int damageMultiplier = 1;

	[Header("Enemy Health")]
	public float basePower = 1f;

	public float floorInfluence = 1f;

	public int floor;

	public float loopPower = 0.2f;

	private void Awake()
	{
		Instance = this;
	}

	public float enemyPower()
	{
		int num = 1;
		if (FloorManager.Instance != null)
		{
			num = FloorManager.Instance.currentLoop;
		}
		return (basePower + floorInfluence * (float)(floor + (num - 1) * FloorManager.Instance.floors.Count)) * Mathf.Pow(num, loopPower);
	}
}
