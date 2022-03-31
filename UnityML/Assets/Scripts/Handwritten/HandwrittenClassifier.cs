using System;
using System.Linq;
using UnityEngine;

namespace ML.Handwritten
{
	public class HandwrittenClassifier : MonoBehaviour
    {
        [SerializeField] private int _inputSize;
        [SerializeField] private int _classSize;
        [SerializeField] private int _hiddenSize;
        [SerializeField] private int _hiddenLayers;
        [SerializeField] private float _learningRate;

        [SerializeField] private float[] _inputCache;
        private Model _model;

        public float[] Classify(ImageData image) 
        {
            Array.Copy(image.Pixels, _inputCache, image.Pixels.Length);
            return _model.Predict(_inputCache).ToArray();
        }

        private void Awake()
        {
            _inputCache = new float[_inputSize];

            _model = new Model();
            _model.Init(_inputSize, _hiddenLayers, _hiddenSize, _classSize, _learningRate);
        }
    }
}