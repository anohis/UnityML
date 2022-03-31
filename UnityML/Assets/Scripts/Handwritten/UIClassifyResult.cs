using UnityEngine;

namespace ML.Handwritten
{
	public class UIClassifyResult : MonoBehaviour
	{
		[SerializeField] private UIPercentage[] _results;

		public void Show(float[] result) 
		{
			for (int i = 0; i < result.Length; i++)
			{
				_results[i].Show(result[i]);
			}
		}
	}
}