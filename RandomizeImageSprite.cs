using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeImageSprite : MonoBehaviour
{
	public List<Sprite> randomSprites = new List<Sprite>();

	private void Start()
	{
		GetComponent<Image>().sprite = randomSprites[Random.Range(0, randomSprites.Count)];
	}
}
