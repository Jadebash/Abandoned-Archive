public class EventRoomLever : Interactable
{
	public delegate void LeaveCallback();

	public bool doesLeave;

	public event LeaveCallback OnLeave;

	public void Leave()
	{
		this.OnLeave?.Invoke();
	}
}
