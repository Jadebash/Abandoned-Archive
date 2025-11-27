using Unity.Services.Analytics;

public class BossStartEvent : Event
{
	public string Boss
	{
		set
		{
			SetParameter("boss", value);
		}
	}

	public BossStartEvent()
		: base("boss_start")
	{
	}
}
