using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace ML.Handwritten
{
	[RequireComponent(typeof(RawImage))]
	public class Board : MonoBehaviour
	{
		private const byte MaxColor = byte.MaxValue;

		public event Action OnChange;

		[SerializeField] private Vector2Int _boardSize;

		private ImageData _image;
		private Texture2D _texture;

		public Vector2Int Size => _boardSize;
		public ImageData Image => _image;

		public void SetPixel(Vector2Int pixel, byte color)
		{
			_image.SetPixel(pixel, color);

			_texture.SetPixel(pixel, GenerateColor(color));
			_texture.Apply();
		}
		public void SetImage(ImageData image)
		{
			_image = image;

			for (int x = 0; x < image.Width; x++)
			{
				for (int y = 0; y < image.Height; y++)
				{
					var color = GenerateColor(image.GetPixel(x, y));
					_texture.SetPixel(x, y, color);
				}
			}
			_texture.Apply();
		}

		private void Awake()
		{
			_image = new ImageData(Size.x, Size.y);
			_image.Fill(0);

			_texture = new Texture2D(Size.x, Size.y, TextureFormat.RGB24, false);
			_texture.Fill(GenerateColor(0));
			_texture.Apply();

			GetComponent<RawImage>().texture = _texture;
		}
		private void OnValidate()
		{
			GetComponent<RectTransform>().sizeDelta = (Vector2)_boardSize;
		}

		private Color GenerateColor(byte value)
		{
			var v = value / (float)MaxColor;
			return new Color(v, v, v);
		}
	}
}