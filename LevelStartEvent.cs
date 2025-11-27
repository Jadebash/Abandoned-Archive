using Unity.Services.Analytics;

public class LevelStartEvent : Event
{
	public int FloorNum
	{
		set
		{
			SetParameter("floor_num", value);
		}
	}

	public LevelStartEvent()
		: base("level_start")
	{
	}
}
