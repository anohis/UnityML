using System;
using System.Linq;
using UnityEngine;

namespace ML.Handwritten.GAN
{
    public class Trainer : MonoBehaviour
    {
        [SerializeField] private MnistLoader _mnistLoader = null;
        [SerializeField] private HandwrittenGAN _gan = null;
        [Range(0, 9)]
        [SerializeField] private byte _targetNum = 0;
        [SerializeField] private int _trainCountPerUpdate = 10;
        [SerializeField] private int _maxEpoch = 1;
        [SerializeField] private Board _board = null;

        private int _epoch;
        private int _step;

        private void Update()
        {
            if (_epoch >= _maxEpoch)
            {
                enabled = false;
            }

            var trainDatas = _mnistLoader.TrainDataTable[_targetNum];

            var start = _step * _trainCountPerUpdate;
            var end = Mathf.Min(start + _trainCountPerUpdate, trainDatas.Length);

            var metrics = _gan.Train(trainDatas
                                    .Skip(start)
                                    .Take(end - start)
                                    .ToArray());

            _step++;
            if (end >= trainDatas.Length)
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