using UnityEngine;

public class FadeOverTime : MonoBehaviour
{
	public float time = 1.5f;

	public GameObject SpriteRenderer;

	private Color myColour;

	private float colourFade = 1f;

	private float timer;

	private void Start()
	{
		myColour = SpriteRenderer.GetComponent<SpriteRenderer>().color;
		myColour.a = 1f;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= time)
		{
			colourFade -= 0.05f;
			myColour.a = colourFade;
			SpriteRenderer.GetComponent<SpriteRenderer>().color = myColour;
			if (colourFade <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
