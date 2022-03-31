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
		[Range(0f, 1f)]
		[SerializeField] private float _drawColor = 1;
		[SerializeField] private float _drawRadius = 1;

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
					DrawBoard(pixel);
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
		private void DrawBoard(Vector2Int pixel)
		{
			var radius = _drawRadius - 1;
			var minx = (int)Mathf.Max(0, pixel.x - radius);
			var miny = (int)Mathf.Max(0, pixel.y - radius);
			var maxx = (int)Mathf.Min(_board.Size.x - 1, pixel.x + radius);
			var maxy = (int)Mathf.Min(_board.Size.y - 1, pixel.y + radius);
			for (int x = minx; x <= maxx; x++)
			{
				for (int y = miny; y <= maxy; y++)
				{
					_board.SetPixel(x, y, _drawColor, false);
				}
			}
			_board.Apply();
		}
	}
}