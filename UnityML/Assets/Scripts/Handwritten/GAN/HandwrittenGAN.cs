using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tensorflow.Keras.Metrics;
using UnityEngine;

namespace ML.Handwritten.GAN
{
	public class HandwrittenGAN : MonoBehaviour
	{
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
			var image = new ImageData(28, 28);
			Array.Copy(pixels, image.Pixels, pixels.Length);
			return image;
		}

		private void Awake()
		{
			_model = new Model();
			_model.Init();
		}
	}
}