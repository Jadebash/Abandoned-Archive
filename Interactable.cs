using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	public delegate void InteractCallback(Interactable pickup, GameObject player);

	public bool isInteractable = true;

	public Animator canvasAnimator;

	public SpriteRenderer spriteIcon;

	public event InteractCallback OnInteract;

	public void InRange()
	{
		if (isInteractable)
		{
			if (canvasAnimator != null)
			{
				canvasAnimator.SetTrigger("inRange");
			}
			if (spriteIcon != null)
			{
				spriteIcon.material.SetFloat("_Outline", 0.02f);
			}
		}
	}

	public void OutOfRange()
	{
		if (canvasAnimator != null)
		{
			canvasAnimator.SetTrigger("outOfRange");
		}
		if (spriteIcon != null)
		{
			spriteIcon.material.SetFloat("_Outline", 0f);
		}
	}

	public void Interact(GameObject player)
	{
		this.OnInteract?.Invoke(this, player);
	}
}
