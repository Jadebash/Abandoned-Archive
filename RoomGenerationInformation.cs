using UnityEngine;

public class RoomGenerationInformation
{
	public Direction exitDirection;

	public Vector3 exitPosition;

	public int recursion;

	public RoomInfo previousRoom;

	public RoomGenerationInformation(Direction exitDirection, Vector3 exitPosition, int recursion, RoomInfo previousRoom = null)
	{
		this.exitDirection = exitDirection;
		this.exitPosition = exitPosition;
		this.recursion = recursion;
		this.previousRoom = previousRoom;
	}
}
