using System;
using UnityEngine;

namespace ML.Handwritten
{
	public class Trainer : MonoBehaviour
	{
		[SerializeField] private MnistLoader _mnistLoader = null;
		[SerializeField] private HandwrittenClassifier _classifier = null;
		[SerializeField] private int _trainCountPerUpdate = 10;

		private int _step;
		private ImageData[] _trainDataCache;

		private void Awake()
		{
			_trainDataCache = new ImageData[_trainCountPerUpdate];
		}

		private void Update()
		{
			var start = _step * _trainCountPerUpdate;
			var end = Mathf.Min(start + _trainCountPerUpdate, _mnistLoader.TrainDatas.Length);
			Array.Copy(_mnistLoader.TrainDatas, start, _trainDataCache, 0, end - start);

			_classifier.Train(_trainDataCache);

			_step++;
			if (end >= _mnistLoader.TrainDatas.Length) 
			{
				enabled = false;
			}

			_mnistLoader.Random();
		}
	}
}