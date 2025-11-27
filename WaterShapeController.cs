using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterShapeController : MonoBehaviour
{
	[SerializeField]
	private float springStiffness = 0.1f;

	[SerializeField]
	private List<WaterSpring> waterSprings;

	[SerializeField]
	private float dampening = 0.03f;

	public float spread = 0.0005f;

	private int cornersCount = 2;

	public SpriteShapeController spriteShapeController;

	private int waveCount = 9;

	public int movingWave;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		foreach (WaterSpring waterSpring in waterSprings)
		{
			waterSpring.WaveSpringUpdate(springStiffness, dampening);
		}
		UpdateSprings();
		UpdateWaves();
	}

	private void UpdateSprings()
	{
		int count = waterSprings.Count;
		float[] array = new float[count];
		float[] array2 = new float[count];
		for (int i = 0; i < count; i++)
		{
			if (i > 0)
			{
				array[i] = spread * (waterSprings[i].height - waterSprings[i - 1].height);
				waterSprings[i - 1].velocity += array[i];
			}
			if (i < count - 1)
			{
				array2[i] = spread * (waterSprings[i].height - waterSprings[i + 1].height);
				waterSprings[i + 1].velocity += array2[i];
			}
		}
	}

	private void UpdateWaves()
	{
		Spline spline = spriteShapeController.spline;
		spline.GetPointCount();
		for (int i = 0; i < waterSprings.Count; i++)
		{
			spline.SetPosition(point: new Vector3(waterSprings[i].transform.position.x, waterSprings[i].transform.localPosition.y, waterSprings[i].transform.position.z), index: i + cornersCount);
		}
	}

	private void SetWaves()
	{
		spriteShapeController.spline.GetPointCount();
	}
}
