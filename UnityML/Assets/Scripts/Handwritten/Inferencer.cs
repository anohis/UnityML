using UnityEngine;

namespace ML.Handwritten
{
	public class Inferencer : MonoBehaviour
	{
		[SerializeField] private Board _board = null;
		[SerializeField] private HandwrittenClassifier _classifier = null;
		[SerializeField] private UIClassifyResult _result = null;

		private void OnEnable()
		{
			_board.OnChange += OnBoardChange;
		}
		private void OnDisable()
		{
			_board.OnChange -= OnBoardChange;
		}

		private void OnBoardChange()
		{
			var image = _board.Image;
			var result = _classifier.Classify(image);
			_result.Show(result);
		}
	}
}