using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow.Keras.Metrics;
using UnityEngine;

namespace ML.Handwritten.Classifier
{
	public class HandwrittenClassifier : MonoBehaviour
	{
		[SerializeField] private int _inputSize;
		[SerializeField] private int _classSize;
		[SerializeField] private int _hiddenSize;
		[SerializeField] private int _hiddenLayers;
		[SerializeField] private float _learningRate;

		private Model _model;

		public float[] Classify(ImageData image)
		{
			return _model.Predict(image.Pixels).ToArray();
		}

		public IEnumerable<Metric> Train(ImageData[] images)
		{
			var dataSize = images.Length;
			var inputs = new float[dataSize, images[0].Pixels.Length];
			var labels = new byte[dataSize, 1];

			for (int i = 0; i < dataSize; i++)
			{
				SetInput(i, images[i].Pixels);
				labels[i, 0] = images[i].Label;
			}

			return _model.Train(inputs, labels);

			void SetInput(int index, float[] array)
			{
				for (int i = 0; i < array.Length; i++)
				{
					inputs[index, i] = array[i];
				}
			}
		}

		private void Awake()
		{
			_model = new Model();
			_model.Init(_inputSize, _hiddenLayers, _hiddenSize, _classSize, _learningRate);
		}
	}
}