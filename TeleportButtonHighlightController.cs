using UnityEngine;

public class TeleportButtonHighlightController : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private ImageBorderHighlight borderHighlight;

	private Movement playerMovement;

	private bool wasHighlightActive;

	private void Start()
	{
		if (borderHighlight == null)
		{
			borderHighlight = GetComponent<ImageBorderHighlight>();
		}
		if (borderHighlight != null)
		{
			borderHighlight.enabled = false;
		}
	}

	private void Update()
	{
		if (playerMovement == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
			if (gameObject != null)
			{
				playerMovement = gameObject.GetComponent<Movement>();
			}
		}
		if (playerMovement != null && borderHighlight != null)
		{
			bool canTeleportToNextFloor = playerMovement.canTeleportToNextFloor;
			if (canTeleportToNextFloor != wasHighlightActive)
			{
				borderHighlight.enabled = canTeleportToNextFloor;
				wasHighlightActive = canTeleportToNextFloor;
			}
		}
	}
}
