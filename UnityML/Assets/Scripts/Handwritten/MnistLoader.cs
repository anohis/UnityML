using JFun.Gameplay.PS2;
using MNIST.IO;
using System.Linq;
using UnityEngine;

namespace ML.Handwritten
{
	public class MnistLoader : MonoBehaviour
	{
		[SerializeField] private string _labelFilePath;
		[SerializeField] private string _imageFilePath;

		[SerializeField] private Board _board = null;

		[Button(nameof(Next), nameof(Next))]
		[SerializeField] private int _showNextBtn;
		[Button(nameof(Prev), nameof(Prev))]
		[SerializeField] private int _showPrevBtn;
		[Button(nameof(Random), nameof(Random))]
		[SerializeField] private int _showRandomBtn;

		private TestCase[] _testCases;
		private int _currentIndex;

		private TestCase Current => _testCases[_currentIndex];

		public TestCase Next()
		{
			_currentIndex = (_currentIndex + 1) % _testCases.Length;
			Show(Current.Image);
			return Current;
		}
		public TestCase Prev()
		{
			_currentIndex = (_testCases.Length + _currentIndex - 1) % _testCases.Length;
			Show(Current.Image);
			return Current;
		}
		public TestCase Random()
		{
			_currentIndex = UnityEngine.Random.Range(0, _testCases.Length);
			Show(Current.Image);
			return Current;
		}

		private void Awake()
		{
			_testCases = FileReaderMNIST.LoadImagesAndLables(_labelFilePath, _imageFilePath).ToArray();
		}

		private void Show(byte[,] image)
		{
			var width = image.GetLength(0);
			var height = image.GetLength(1);
			var imageData = new ImageData(width, height);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var value = image[image.GetLength(0) - y - 1, x];
					imageData.SetPixel(x, y, value);
				}
			}

			_board.SetImage(imageData);
		}
	}
}