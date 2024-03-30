using System;
using UnityEngine;

public class BarChartItem : MonoBehaviour
{
    public BarChart Parent;

    private RectTransform _rectTransform;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private float _value;
    public float Value
    {
        get { return _value; }
        set
        {
            if (Parent == null)
            {
                throw new InvalidOperationException("BarChartItem Parent not set");
            }

            _value = Mathf.Clamp01(value);

            var height = Parent.ChartInnerHeight * value;
            _spriteRenderer.size = new Vector2(_spriteRenderer.size.x, height);
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
        }
    }

    public float Width
    {
        get { return _spriteRenderer.size.x; }
        set
        {
            _spriteRenderer.size = new Vector2(value, _spriteRenderer.size.y);
            _rectTransform.sizeDelta = new Vector2(value, _rectTransform.sizeDelta.y);
        }
    }

    public Color Color
    {
        get { return _spriteRenderer.color; }
        set { _spriteRenderer.color = value; }
    }
}
