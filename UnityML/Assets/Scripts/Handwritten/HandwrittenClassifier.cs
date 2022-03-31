using UnityEngine;

namespace ML.Handwritten
{
    public class HandwrittenClassifier : MonoBehaviour
    {
        private Model _model;

        public void Classify() 
        {

        }

        private void Awake()
        {
            _model = new Model();
        }
    }
}