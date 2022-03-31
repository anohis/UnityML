using UnityEngine;

namespace ML.Handwritten
{
	public struct ImageData
	{
		public byte Label;
		public int Width { get; private set; }
		public int Height { get; private set; }
		public float[] Pixels { get; private set; }

		public ImageData(int width, int height)
		{
			Width = width;
			Height = height;
			Pixels = new float[width * height];
			Label = default;
		}

		public float GetPixel(int x, int y)
		{
			var index = GetPixelIndex(x, y);
			return Pixels[index];
		}
		public void SetPixel(int x, int y, float value)
		{
			var index = GetPixelIndex(x, y);
			Pixels[index] = value;
		}
		public void SetPixel(Vector2Int pos, float value)
		{
			SetPixel(pos.x, pos.y, value);
		}

		public void Fill(float value)
		{
			for (int i = 0; i < Pixels.Length; i++)
			{
				Pixels[i] = value;
			}
		}

		private int GetPixelIndex(int x, int y)
		{
			return y * Width + x;
		}
		private Vector2Int GetPixelPos(int index)
		{
			return new Vector2Int(index % Width, index / Width);
		}
	}
}