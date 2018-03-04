using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype
{
	public static class ExtensionMethods
	{
		public static Vector3 ToVector3(this Vector2 vec)
		{
			return new Vector3(vec.x, 0f, vec.y);
		}

		public static void SetTexture(this Image image, Texture2D texture)
		{
			image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}
	}
}