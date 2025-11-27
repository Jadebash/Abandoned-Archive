using UnityEngine;

public class PlayButton : MonoBehaviour
{
	public void Play()
	{
		Manager.Instance?.Play();
	}
}
