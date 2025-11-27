using Unity.Services.Analytics;

public class LevelCompleteEvent : Event
{
	public int FloorNum
	{
		set
		{
			SetParameter("floor_num", value);
		}
	}

	public LevelCompleteEvent()
		: base("level_complete")
	{
	}
}
