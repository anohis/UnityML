using UnityEngine;

namespace ML.Handwritten
{
    public class HandwrittenClassifier : MonoBehaviour
    {
        private Model _model;

        private void Awake()
        {
            _model = new Model();
        }
    }
}