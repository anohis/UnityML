using System;
using System.Collections.Generic;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;
using static Tensorflow.KerasApi;

namespace ML.Handwritten
{
    public class Model
    {
        private int _inputNode;
        private int _outputNode;
        private Functional _model;

        public void Init(int inputNode, int hiddenLayer, int hiddenNode, int outputNode, float learnRate)
        {
            _inputNode = inputNode;
            _outputNode = outputNode;

            var layers = keras.layers;

            var inputs = keras.Input(shape: inputNode, name: "img");
            var hidden = inputs;
            for (int i = 0; i < hiddenLayer; i++)
            {
                hidden = layers.Dense(hiddenNode, activation: "relu").Apply(hidden);
            }
            var output = layers.Dense(outputNode, activation: "relu").Apply(hidden);
            output = layers.Softmax(-1).Apply(output);

            _model = keras.Model(inputs, output, name: "model");
            _model.compile(optimizer: keras.optimizers.SGD(learnRate), loss: keras.losses.SparseCategoricalCrossentropy());
        }

        public IEnumerable<float> Predict(float[] input)
        {
            if (input.Length != _inputNode) 
            {
                throw new ArgumentException("input.Length Error");
            }

            var x = np.array(input);
            var predict = _model.predict(x);
            for (int i = 0; i < _outputNode; i++) 
            {
                yield return (float)predict[i];
            }
        }
    }
}