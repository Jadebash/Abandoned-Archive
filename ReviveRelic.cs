using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "Abandoned Archive/Relics/Revive Relic")]
public class ReviveRelic : Relic
{
	[HideInInspector]
	private Dictionary<GameObject, bool> playerUsageStatus = new Dictionary<GameObject, bool>();

	public override void GainedRelic(GameObject player)
	{
		if (!playerUsageStatus.ContainsKey(player))
		{
			playerUsageStatus[player] = false;
		}
		Death.Instance.OnDeath += Revive;
	}

	public override void LostRelic(GameObject player)
	{
		if (playerUsageStatus.ContainsKey(player))
		{
			playerUsageStatus.Remove(player);
		}
		Death.Instance.OnDeath -= Revive;
	}

	public DeathOverride Revive()
	{
		GameObject gameObject = null;
		GameObject[] players = PlayerManager.players;
		foreach (GameObject gameObject2 in players)
		{
			if (gameObject2 != null && gameObject2.GetComponent<Health>() != null && gameObject2.GetComponent<Health>().health <= 0f)
			{
				gameObject = gameObject2;
				break;
			}
		}
		if (gameObject != null && playerUsageStatus.ContainsKey(gameObject) && !playerUsageStatus[gameObject])
		{
			playerUsageStatus[gameObject] = true;
			Use();
			RuntimeManager.PlayOneShot("event:/SFX/Main Character/Revive");
			RelicCollector.Instance.DropRelic(this);
			return DeathOverride.ExtraLife;
		}
		return DeathOverride.None;
	}
}
