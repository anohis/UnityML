using JFun.Gameplay.PS2;
using UnityEngine;

namespace ML.Handwritten.GAN
{
	public class Inferencer : MonoBehaviour
	{
		[SerializeField] private Board _board = null;
		[SerializeField] private HandwrittenGAN _gan = null;

		[Button(nameof(Generate), nameof(Generate))]
		[SerializeField] private int _generateBtn;

		private void Generate()
		{
			var image = _gan.Generate();
			_board.SetImage(image);
		}
	}
}