using UnityEngine;

public class SirVeltrineCardTypeBehaviour : MonoBehaviour
{
	public GameObject coins;

	public GameObject fire;

	public GameObject healVeltrine;

	public GameObject healPlayer;

	public GameObject spawnEnemy;

	public void EnableIcons(SirVeltrineCardType type)
	{
		switch (type)
		{
		case SirVeltrineCardType.Coins:
			coins.SetActive(value: true);
			break;
		case SirVeltrineCardType.Fire:
			fire.SetActive(value: true);
			break;
		case SirVeltrineCardType.HealVeltrine:
			healVeltrine.SetActive(value: true);
			break;
		case SirVeltrineCardType.HealPlayer:
			healPlayer.SetActive(value: true);
			break;
		case SirVeltrineCardType.SpawnEnemy:
			spawnEnemy.SetActive(value: true);
			break;
		}
	}

	public void DisableCard()
	{
		coins.SetActive(value: false);
		fire.SetActive(value: false);
		healVeltrine.SetActive(value: false);
		healPlayer.SetActive(value: false);
		spawnEnemy.SetActive(value: false);
		GetComponent<Animator>().enabled = false;
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
	}
}
