using JFun.Gameplay.PS2;
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
		public event Action OnChange;

		[SerializeField] private Vector2Int _boardSize;
		[Button(nameof(Clear), nameof(Clear))]
		[SerializeField] private int _clearBtn;
		[SerializeField] private float[] _pixels;

		private ImageData _image;
		private Texture2D _texture;

		public Vector2Int Size => _boardSize;
		public ImageData Image => _image;

		public void Apply()
		{
			_pixels = _image.Pixels;

			_texture.Apply();
			OnChange?.Invoke();
		}
		public void SetPixel(int x, int y, float color, bool autoApply = true)
		{
			_image.SetPixel(x, y, color);

			_texture.SetPixel(x, y, GenerateColor(color));

			if (autoApply)
			{
				Apply();
			}
		}
		public void SetPixel(Vector2Int pixel, float color)
		{
			SetPixel(pixel.x, pixel.y, color);
		}
		public void SetImage(ImageData image)
		{
			Array.Copy(image.Pixels, _image.Pixels, _image.Pixels.Length);

			for (int x = 0; x < image.Width; x++)
			{
				for (int y = 0; y < image.Height; y++)
				{
					var color = GenerateColor(image.GetPixel(x, y));
					_texture.SetPixel(x, y, color);
				}
			}

			Apply();
		}

		private void Awake()
		{
			_image = new ImageData(Size.x, Size.y);
			_image.Fill(0);

			_texture = new Texture2D(Size.x, Size.y, TextureFormat.RGB24, false);
			_texture.Fill(GenerateColor(0));
			Apply();

			GetComponent<RawImage>().texture = _texture;
		}
		private void OnValidate()
		{
			GetComponent<RectTransform>().sizeDelta = (Vector2)_boardSize;
		}

		private Color GenerateColor(float value)
		{
			return new Color(value, value, value);
		}

		private void Clear()
		{
			_image.Fill(0);
			_texture.Fill(GenerateColor(0));
			Apply();
		}
	}
}