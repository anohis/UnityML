using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using Tensorflow.Gradients;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Losses;
using Tensorflow.Keras.Metrics;
using Tensorflow.Keras.Optimizers;
using Tensorflow.Keras.Utils;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace ML.Handwritten.GAN
{
	public class Model
	{
		private int _inputSize;
		private int _outputSize;

		private Functional _generator;
		private Functional _discriminator;
		private OptimizerV2 _optimizer;
		private ILossFunc _loss;

		private List<Metric> _metrics;

		public void Init()
		{
			_inputSize = 128;
			_outputSize = 784;

			_generator = GenerateModel(_inputSize, 1, 256, _outputSize);
			_discriminator = GenerateModel(_outputSize, 1, 256, 1);
			_optimizer = keras.optimizers.RMSprop(0.001f);
			_loss = keras.losses.MeanSquaredError();

			_metrics = new List<Metric>
			{
				new MeanMetricWrapper((x, y) => _loss.Call(x, y), "generator_loss"),
				new MeanMetricWrapper(keras.metrics.binary_accuracy, "generator_acc"),
				new MeanMetricWrapper((x, y) => _loss.Call(x, y), "discriminator_loss"),
				new MeanMetricWrapper(keras.metrics.binary_accuracy, "discriminator_acc"),
			};
		}

		public IEnumerable<Metric> Train(float[,] input)
		{
			var dataLen = input.GetLength(0);
			var trainDataLen = dataLen * 2;

			var label = tf.ones((trainDataLen, 1), tf.float32);
			Tensors generatorData = null;

			using (var gradientTape = tf.GradientTape())
			{
				generatorData = _generator.Apply(tf.random.normal((trainDataLen, _inputSize)), training: true);
				var predict = _discriminator.Apply(generatorData, training: false);
				var loss = _loss.Call(label, predict);
				var grads = gradientTape.gradient(loss, _generator.trainable_variables);
				var z = zip(grads, _generator.trainable_variables.Select(x => x as ResourceVariable));
				_optimizer.apply_gradients(z);

				_metrics[0].update_state(label, predict);
				_metrics[1].update_state(label, predict);
			}

			label = tf.concat(new Tensor[]
							{
								tf.ones((dataLen, 1), tf.float32),
								tf.zeros((dataLen, 1), tf.float32)
							}, axis: 0);

			var x = tf.concat(new Tensor[]
							{
								np.array(input),
								generatorData[0][new Slice(stop: dataLen)],
							}, axis: 0);

			using (var gradientTape = tf.GradientTape())
			{
				var predict = _discriminator.Apply(x, training: true);
				var loss = _loss.Call(label, predict);
				var grads = gradientTape.gradient(loss, _discriminator.trainable_variables);
				var z = zip(grads, _discriminator.trainable_variables.Select(x => x as ResourceVariable));
				_optimizer.apply_gradients(z);

				_metrics[2].update_state(label, predict);
				_metrics[3].update_state(label, predict);
			}

			return _metrics;
		}
		public IEnumerable<float> Predict()
		{
			var predict = _generator.predict(tf.random.normal((1, _inputSize)))[0].numpy();

			for (int i = 0; i < _outputSize; i++)
			{
				yield return predict[0, i];
			}
		}

		private Functional GenerateModel(int inputNode, int hiddenLayer, int hiddenNode, int outputNode)
		{
			var layers = keras.layers;

			var inputs = keras.Input(shape: inputNode);
			var hidden = inputs;
			for (int i = 0; i < hiddenLayer; i++)
			{
				hidden = layers.Dense(hiddenNode, activation: "relu").Apply(hidden);
			}
			var output = layers.Dense(outputNode, activation: "sigmoid").Apply(hidden);

			return keras.Model(inputs, output);
		}
	}
}