using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MNIST.IO;
using JFun.Gameplay.PS2;
using System.Linq;

namespace ML.Handwritten
{
	public class MnistLoader : MonoBehaviour
	{
		[SerializeField] private string _labelFilePath;
		[SerializeField] private string _imageFilePath;

		[SerializeField] private Board _board = null;

		[Button(nameof(ShowNext), nameof(ShowNext))]
		[SerializeField] private int _showNextBtn;
		[Button(nameof(ShowPrev), nameof(ShowPrev))]
		[SerializeField] private int _showPrevBtn;

		private TestCase[] _testCases;
		private int _currentIndex;

		private void Awake()
		{
			_testCases = FileReaderMNIST.LoadImagesAndLables(_labelFilePath, _imageFilePath).ToArray();
		}

		private void ShowNext()
		{
			_currentIndex = (_currentIndex + 1) % _testCases.Length;
			ShowCurrent();
		}
		private void ShowPrev()
		{
			_currentIndex = (_testCases.Length + _currentIndex - 1) % _testCases.Length;
			ShowCurrent();
		}
		private void ShowCurrent()
		{
			var testCase = _testCases[_currentIndex];
			_board.SetPixels(ForEachColors(testCase.Image));

			IEnumerable<(Vector2Int, Color)> ForEachColors(byte[,] image)
			{
				for (int x = 0; x < image.GetLength(0); x++)
				{
					for (int y = 0; y < image.GetLength(1); y++)
					{
						var c = image[image.GetLength(0) - y - 1, x];
						yield return (new Vector2Int(x, y), new Color(c, c, c));
					}
				}
			}
		}
	}
}