using UnityEngine;

[ExecuteInEditMode]
public class SetMaterialSpriteProperties : MonoBehaviour
{
	private static readonly int ID_Min = Shader.PropertyToID("_SpriteUVMin");

	private static readonly int ID_Max = Shader.PropertyToID("_SpriteUVMax");

	private static readonly int ID_Aspect = Shader.PropertyToID("_Aspect");

	private SpriteRenderer sr;

	private MaterialPropertyBlock mpb;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		mpb = new MaterialPropertyBlock();
	}

	private void OnWillRenderObject()
	{
		if ((bool)sr && (bool)sr.sprite)
		{
			Sprite sprite = sr.sprite;
			Texture2D texture = sprite.texture;
			Rect textureRect = sprite.textureRect;
			Vector2 vector = new Vector2(textureRect.xMin / (float)texture.width, textureRect.yMin / (float)texture.height);
			Vector2 vector2 = new Vector2(textureRect.xMax / (float)texture.width, textureRect.yMax / (float)texture.height);
			float value = textureRect.width / textureRect.height;
			sr.GetPropertyBlock(mpb);
			mpb.SetVector(ID_Min, vector);
			mpb.SetVector(ID_Max, vector2);
			mpb.SetFloat(ID_Aspect, value);
			sr.SetPropertyBlock(mpb);
		}
	}
}
