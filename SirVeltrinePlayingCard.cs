using UnityEngine;

public class SirVeltrinePlayingCard : MonoBehaviour
{
	public SirVeltrineCardType cardType;

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			base.transform.parent.parent.GetComponent<SirVeltrine>().ChooseCard(this);
		}
	}
}
