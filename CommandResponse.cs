public class CommandResponse
{
	public bool success;

	public string response;

	public CommandResponse(bool success, string response)
	{
		this.success = success;
		this.response = response;
	}
}
