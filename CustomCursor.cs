using UnityEngine;

public class CustomCursor : MonoBehaviour
{
	public Texture2D cursor;

	private void Start()
	{
		if (cursor != null)
		{
			Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
		}
	}
}
