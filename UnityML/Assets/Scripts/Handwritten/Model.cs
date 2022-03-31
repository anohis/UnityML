using System;
using System.Collections.Generic;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Metrics;
using Tensorflow.NumPy;
using static Tensorflow.KerasApi;
using static Tensorflow.Binding;
using System.IO;
using Tensorflow.Keras.Utils;

namespace ML.Handwritten
{
	public class Model
	{
		private int _inputNode;
		private int _outputNode;
		private Functional _model;

		private float[,] _inputCache;
		private float[] _labelCache;

		public IEnumerable<Metric> Metrics => _model.metrics;

		public void Init(int inputNode, int hiddenLayer, int hiddenNode, int outputNode, float learnRate)
		{
			_inputNode = inputNode;
			_outputNode = outputNode;

			_inputCache = new float[1, inputNode];
			_labelCache = new float[outputNode];

			var layers = keras.layers;

			var inputs = keras.Input(shape: inputNode, name: "img");
			var hidden = inputs;
			for (int i = 0; i < hiddenLayer; i++)
			{
				hidden = layers.Dense(hiddenNode, activation: "relu").Apply(hidden);
			}
			var output = layers.Dense(outputNode).Apply(hidden);
			output = layers.Softmax(-1).Apply(output);

			_model = keras.Model(inputs, output, name: "model");

			var optimizer = keras.optimizers.RMSprop(learnRate);
			var loss = keras.losses.CategoricalCrossentropy();
			var metrics = new string[] { "acc" };
			_model.compile(optimizer, loss, metrics);
		}

		public void Train(float[,] input, byte[,] label, int batch_size, int epochs)
		{
			var x = np.array(input);
			var y = np_utils.to_categorical(label, _outputNode);
			_model.fit(x, y, batch_size, epochs);
		}
		public IEnumerable<float> Predict(float[] input)
		{
			if (input.Length != _inputNode)
			{
				throw new ArgumentException("input.Length Error");
			}

			var x = GenerateInputArray(input);
			var predict = _model.predict(x)[0].numpy();

			for (int i = 0; i < _outputNode; i++)
			{
				yield return predict[0, i];
			}
		}

		private NDArray GenerateInputArray(float[] input)
		{
			for (int i = 0; i < _inputNode; i++)
			{
				_inputCache[0, i] = input[i];
			}
			return np.array(_inputCache);
		}
		private NDArray GenerateLabelArray(byte label)
		{
			return np_utils.to_categorical(label, _outputNode);
		}
	}
}