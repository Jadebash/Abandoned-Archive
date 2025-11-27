using UnityEngine;

public class OrbHeadController : MonoBehaviour
{
	public OrbHead orbHead;

	public void ThrowHead()
	{
		orbHead.SpawnProjectile();
	}
}
