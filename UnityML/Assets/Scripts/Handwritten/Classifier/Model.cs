using System;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Losses;
using Tensorflow.Keras.Metrics;
using Tensorflow.Keras.Optimizers;
using Tensorflow.Keras.Utils;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace ML.Handwritten.Classifier
{
	public class Model
	{
		private int _inputNode;
		private int _outputNode;

		private Functional _model;
		private OptimizerV2 _optimizer;
		private ILossFunc _loss;
		private List<Metric> _metrics;

		private float[,] _inputCache;

		public void Init(int inputNode, int hiddenLayer, int hiddenNode, int outputNode, float learnRate)
		{
			_inputNode = inputNode;
			_outputNode = outputNode;

			_inputCache = new float[1, inputNode];

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

			_optimizer = keras.optimizers.RMSprop(learnRate);
			_loss = keras.losses.CategoricalCrossentropy(from_logits: true);

			_metrics = new List<Metric>
			{
				new MeanMetricWrapper((x, y) => _loss.Call(x, y), "loss"),
				new MeanMetricWrapper(keras.metrics.categorical_accuracy, "acc"),
			};
		}

		public IEnumerable<Metric> Train(float[,] input, byte[,] label)
		{
			var x = np.array(input);
			var y = np_utils.to_categorical(label, _outputNode);
			var g = tf.GradientTape();
			var predict = _model.predict(x, (int)x.shape[0]);
			var loss = _loss.Call(y, predict);
			var grads = g.gradient(loss, _model.trainable_variables);
			var z = zip(grads, _model.trainable_variables.Select(x => x as ResourceVariable));
			_optimizer.apply_gradients(z);

			_metrics[0].update_state(y, predict);
			_metrics[1].update_state(y, predict);

			return _metrics;
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
	}
}