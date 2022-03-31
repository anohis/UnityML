using UnityEngine;
using UnityEngine.UI;

namespace ML.Handwritten
{
	public class Inferencer : MonoBehaviour
	{
		[SerializeField] private Board _board = null;
		[SerializeField] private HandwrittenClassifier _classifier = null;
		[SerializeField] private UIPercentage[] _results;

		private void Awake()
		{
			_board.OnChange += OnBoardChange;
		}
		private void OnDestroy()
		{
			_board.OnChange -= OnBoardChange;
		}

		private void OnBoardChange()
		{
			var image = _board.Image;
			var result = _classifier.Classify(image);
			for (int i = 0; i < result.Length; i++) 
			{
				_results[i].Show(result[i]);
			}
		}
	}
}