using System;
using UnityEngine;

namespace ML.Handwritten.GAN
{
	public class Trainer : MonoBehaviour
	{
		[SerializeField] private MnistLoader _mnistLoader = null;
		[SerializeField] private HandwrittenGAN _gan = null;
		[SerializeField] private int _trainCountPerUpdate = 10;
		[SerializeField] private int _maxEpoch = 1;
		[SerializeField] private Board _board = null;

		private int _epoch;
		private int _step;
		private ImageData[] _trainDataCache;

		private void Awake()
		{
			_trainDataCache = new ImageData[_trainCountPerUpdate];
		}

		private void Update()
		{
			if (_epoch >= _maxEpoch)
			{
				enabled = false;
			}

			var start = _step * _trainCountPerUpdate;
			var end = Mathf.Min(start + _trainCountPerUpdate, _mnistLoader.TrainDatas.Length);
			Array.Copy(_mnistLoader.TrainDatas, start, _trainDataCache, 0, end - start);

			var metrics = _gan.Train(_trainDataCache);

			_step++;
			if (end >= _mnistLoader.TrainDatas.Length)
			{
				_step = 0;
				_epoch++;

				var str = $"epoch: {_epoch}, ";
				foreach (var v in metrics)
				{
					str += $"{v.Name}: {(float)(v.result().numpy())}, ";
					v.reset_states();
				}
				
				Debug.LogError(str);
			}

			var image = _gan.Generate();
			_board.SetImage(image);
		}
	}
}