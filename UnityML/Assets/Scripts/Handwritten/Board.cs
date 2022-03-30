using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ML.Handwritten
{
	[RequireComponent(typeof(RawImage))]
	public class Board : MonoBehaviour
	{
		[SerializeField] private Vector2Int _boardSize;

		private Texture2D _texture;

		public Vector2Int Size => _boardSize;

		public void SetPixel(Vector2Int pixel, Color color)
		{
			_texture.SetPixel(pixel, color);
			_texture.Apply();
		}

		public void SetPixels(IEnumerable<(Vector2Int, Color)> pixels)
		{
			foreach (var v in pixels)
			{
				_texture.SetPixel(v.Item1, v.Item2);
			}
			_texture.Apply();
		}

		private void Awake()
		{
			_texture = new Texture2D(Size.x, Size.y, TextureFormat.ARGB32, false);
			_texture.Fill(Color.white);
			_texture.Apply();

			GetComponent<RawImage>().texture = _texture;
		}

		private void OnValidate()
		{
			GetComponent<RectTransform>().sizeDelta = (Vector2)_boardSize;
		}
	}
}