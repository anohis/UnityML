using UnityEngine;

namespace ML.Handwritten
{
	public struct ImageData
	{
		public int Width { get; private set; }
		public int Height { get; private set; }
		public byte[] Pixels { get; private set; }

		public ImageData(int width, int height)
		{
			Width = width;
			Height = height;
			Pixels = new byte[width * height];
		}

		public byte GetPixel(int x, int y)
		{
			var index = GetPixelIndex(x, y);
			return Pixels[index];
		}
		public void SetPixel(int x, int y, byte value)
		{
			var index = GetPixelIndex(x, y);
			Pixels[index] = value;
		}
		public void SetPixel(Vector2Int pos, byte value)
		{
			SetPixel(pos.x, pos.y, value);
		}

		public void Fill(byte value)
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