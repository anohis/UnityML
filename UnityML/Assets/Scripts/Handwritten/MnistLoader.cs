using JFun.Gameplay.PS2;
using MNIST.IO;
using System.Collections.Generic;
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

        private int _currentIndex;

        public ImageData[] TrainDatas { get; private set; }
        public IReadOnlyDictionary<byte, ImageData[]> TrainDataTable { get; private set; }

        private ImageData Current => TrainDatas[_currentIndex];

        public ImageData Next()
        {
            _currentIndex = (_currentIndex + 1) % TrainDatas.Length;
            Show(Current);
            return Current;
        }
        public ImageData Prev()
        {
            _currentIndex = (TrainDatas.Length + _currentIndex - 1) % TrainDatas.Length;
            Show(Current);
            return Current;
        }
        public ImageData Random()
        {
            _currentIndex = UnityEngine.Random.Range(0, TrainDatas.Length);
            Show(Current);
            return Current;
        }

        private void Awake()
        {
            TrainDatas = FileReaderMNIST.LoadImagesAndLables(_labelFilePath, _imageFilePath)
                        .Select(x => GenerateImageData(x))
                        .ToArray();

            TrainDataTable = TrainDatas
                .GroupBy(x => x.Label)
                .ToDictionary(x => x.Key, x => x.ToArray());

            ImageData GenerateImageData(TestCase testCase)
            {
                var image = testCase.Image;
                var width = image.GetLength(0);
                var height = image.GetLength(1);
                var imageData = new ImageData(width, height);
                imageData.Label = testCase.Label;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var value = image[image.GetLength(0) - y - 1, x];
                        imageData.SetPixel(x, y, value / 255f);
                    }
                }
                return imageData;
            }
        }

        private void Show(ImageData image)
        {
            _board.SetImage(image);
        }
    }
}