using Unity.Services.Analytics;

public class PlayerDamageEvent : Event
{
	public string Attacker
	{
		set
		{
			SetParameter("attacker", value);
		}
	}

	public int Damage
	{
		set
		{
			SetParameter("damage", value);
		}
	}

	public PlayerDamageEvent()
		: base("player_damage")
	{
	}
}
