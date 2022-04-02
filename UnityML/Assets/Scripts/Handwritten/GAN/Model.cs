using System.Collections.Generic;
using System.Linq;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Losses;
using Tensorflow.Keras.Metrics;
using Tensorflow.Keras.Optimizers;
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

        public void Init(int inputSize, int outputSize)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;

            GenerateGenerator(_inputSize, 1, 256, _outputSize);
            GenerateDiscriminator(_outputSize, 1, 256, 1);
            _optimizer = keras.optimizers.RMSprop(0.001f);
            _loss = new BinaryCrossentropy();

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

        private void GenerateGenerator(int inputSize, int hiddenLayer, int hiddenSize, int outputSize)
        {
            var layers = keras.layers;

            var inputs = keras.Input(shape: inputSize);
            var hidden = inputs;
            for (int i = 0; i < hiddenLayer; i++)
            {
                hidden = layers.Dense(hiddenSize, activation: "relu").Apply(hidden);
            }
            var output = layers.Dense(outputSize, activation: "relu").Apply(hidden);

            _generator = keras.Model(inputs, output);
        }
        private void GenerateDiscriminator(int inputSize, int hiddenLayer, int hiddenSize, int outputSize)
        {
            var layers = keras.layers;

            var inputs = keras.Input(shape: inputSize);
            var hidden = inputs;
            for (int i = 0; i < hiddenLayer; i++)
            {
                hidden = layers.Dense(hiddenSize, activation: "relu").Apply(hidden);
            }
            var output = layers.Dense(outputSize, activation: "sigmoid").Apply(hidden);

            _discriminator = keras.Model(inputs, output);
        }
    }
}