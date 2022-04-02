using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow.Keras.Metrics;
using UnityEngine;

namespace ML.Handwritten.GAN
{
    public class HandwrittenGAN : MonoBehaviour
    {
        [SerializeField] private int _inputSize = 32;
        [SerializeField] private Vector2Int _imageSize = new Vector2Int(28, 28);

        private Model _model;

        public IEnumerable<Metric> Train(ImageData[] images)
        {
            var dataSize = images.Length;
            var inputs = new float[dataSize, images[0].Pixels.Length];

            for (int i = 0; i < dataSize; i++)
            {
                SetInput(i, images[i].Pixels);
            }

            return _model.Train(inputs);

            void SetInput(int index, float[] array)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    inputs[index, i] = array[i];
                }
            }
        }
        public ImageData Generate()
        {
            var pixels = _model.Predict().ToArray();
            var image = new ImageData(_imageSize.x, _imageSize.y);
            Array.Copy(pixels, image.Pixels, pixels.Length);
            return image;
        }

        private void Awake()
        {
            _model = new Model();
            _model.Init(_inputSize, _imageSize.x * _imageSize.y);
        }
    }
}