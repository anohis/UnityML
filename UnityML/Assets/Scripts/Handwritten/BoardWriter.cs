using UnityEngine;
using UnityEngine.UI;
using Utility;
using Utility.Math;

namespace ML.Handwritten
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RawImage))]
    public class BoardWriter : MonoBehaviour
    {
        [SerializeField] private Board _board;
        [Min(1f)]
        [SerializeField] private float _scale = 1;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _highlightColor;
        [SerializeField] private byte _drawColor;

        private Texture2D _texture;
        private Bounds2D _bounds;

        private Vector2Int _currentPixel;

        private void Awake()
        {
            _bounds = GetComponent<RectTransform>().GetBounds();

            _texture = new Texture2D(_board.Size.x, _board.Size.y, TextureFormat.ARGB32, false);
            _texture.Fill(_normalColor);
            _texture.Apply();

            GetComponent<RawImage>().texture = _texture;
        }
        private void OnValidate()
        {
            if (_board != null)
            {
                GetComponent<RectTransform>().sizeDelta = (Vector2)_board.Size * _scale;
                _board.GetComponent<RectTransform>().localScale = new Vector3(_scale, _scale, _scale);
            }
        }
        private void Update()
        {
            var mousePos = Input.mousePosition.XY();
            if (_bounds.IsContain(mousePos))
            {
                var pixel = ToPixel(mousePos);
                HighlightPixel(pixel);
                if (Input.GetMouseButton(0))
                {
                    _board.SetPixel(pixel, _drawColor);
                }
            }

            Vector2Int ToPixel(Vector2 pos)
            {
                var localPos = _bounds.ToLocalPos(pos) / _scale;
                return new Vector2Int((int)localPos.x, (int)localPos.y);
            }
        }

        private void HighlightPixel(Vector2Int pixel)
        {
            if (pixel != _currentPixel)
            {
                _texture.SetPixel(_currentPixel, _normalColor);
                _currentPixel = pixel;
                _texture.SetPixel(_currentPixel, _highlightColor);
                _texture.Apply();
            }
        }
    }
}