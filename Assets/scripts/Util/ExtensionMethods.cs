using UnityEngine;
using UnityEngine.UI;

namespace DistributionPrototype
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Converts a <see cref="Vector2"/> to <see cref="Vector3"/> using
		/// the x and y axes (default Unity conversion will use x and z).
		/// </summary>
		/// <param name="vec">Vector2 struct to convert</param>
		/// <returns>Converted Vector3 struct</returns>
		public static Vector3 ToVector3(this Vector2 vec)
		{
			return new Vector3(vec.x, 0f, vec.y);
		}

		/// <summary>
		/// Creates a new sprite using the given <paramref name="texture"/> and
		/// assigns it to the given <paramref name="image"/>.
		/// </summary>
		/// <param name="image">Image to set the texture to</param>
		/// <param name="texture">Texture to be referenced by a new sprite</param>
		public static void SetTexture(this Image image, Texture2D texture)
		{
			image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}
	}
}