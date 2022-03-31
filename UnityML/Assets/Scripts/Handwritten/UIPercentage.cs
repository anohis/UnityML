using UnityEngine;
using UnityEngine.UI;

namespace ML.Handwritten
{
	public class UIPercentage : MonoBehaviour
    {
        [SerializeField] private Image _image = null;
        [SerializeField] private Text _value = null;

        public void Show(float value) 
        {
            _image.fillAmount = value;
            _value.text = value.ToString(".00%");
        }
    }
}