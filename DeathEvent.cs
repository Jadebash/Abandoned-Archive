using Unity.Services.Analytics;

public class DeathEvent : Event
{
	public string Attacker
	{
		set
		{
			SetParameter("attacker", value);
		}
	}

	public int Time
	{
		set
		{
			SetParameter("time", value);
		}
	}

	public DeathEvent()
		: base("death")
	{
	}
}
