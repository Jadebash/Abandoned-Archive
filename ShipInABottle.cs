using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Ship in a Bottle")]
public class ShipInABottle : Relic
{
	private const float defaultX = 0.3f;

	private const float defaultY = 0.55f;

	private float prevBoundaryCheckRadius;

	public override void GainedRelic(GameObject player)
	{
		Use();
		float x = player.GetComponent<CapsuleCollider2D>().size.x;
		float y = player.GetComponent<CapsuleCollider2D>().size.y;
		player.GetComponent<CapsuleCollider2D>().size = new Vector3(x / 1.5f, y / 1.5f);
		prevBoundaryCheckRadius = player.GetComponent<Movement>().boundaryCheckRadius;
		player.GetComponent<Movement>().boundaryCheckRadius = 0.05f;
	}

	public override void LostRelic(GameObject player)
	{
		player.GetComponent<CapsuleCollider2D>().size = new Vector3(0.3f, 0.55f);
		player.GetComponent<Movement>().boundaryCheckRadius = prevBoundaryCheckRadius;
	}
}
