using Unity.Services.Analytics;

public class BossEndEvent : Event
{
	public string Boss
	{
		set
		{
			SetParameter("boss", value);
		}
	}

	public BossEndEvent()
		: base("boss_end")
	{
	}
}
