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

		private ImageData _image;
		private Texture2D _texture;

		public Vector2Int Size => _boardSize;
		public ImageData Image => _image;

		public void SetPixel(Vector2Int pixel, float color)
		{
			_image.SetPixel(pixel, color);

			_texture.SetPixel(pixel, GenerateColor(color));
			_texture.Apply();

			OnChange?.Invoke();
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
			_texture.Apply();

			OnChange?.Invoke();
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

		private Color GenerateColor(float value)
		{
			return new Color(value, value, value);
		}

		private void Clear() 
		{
			_image.Fill(0);
			_texture.Fill(GenerateColor(0));
			_texture.Apply();
		}
	}
}