using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class BarChart : MonoBehaviour
{

    public float YMin = 0.0f;
    public float YMax = 100.0f;
    public float BarWidth = 0.0f;

    [SerializeField]
    private float _xOffset = 0.0f;
    public float XOffset
    {
        get { return _xOffset; }
        set
        {
            _xOffset = value;
            ChartInner.anchoredPosition = new Vector2(_xOffset * -BarWidth, ChartInner.anchoredPosition.y);
        }
    }

    public Text TxtYMin;
    public Text TxtYMax;
    public string AxisFormat = "0.0";
    public RectTransform ChartInner;

    private float[] _values;

    private List<BarChartItem> _valueBars = new();

    public BarChartItem BarItemPrefab;
    public Color MinColor = Color.grey;
    public Color MaxColor = Color.white;

    public float ChartInnerHeight
    {
        get
        {
            return ChartInner.rect.height;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       // SetYAxis(YMin, YMax);
    }

    public void SetYAxis(float min, float max)
    {
        YMin = min;
        YMax = max;
        var format = "{0:" + AxisFormat + "}";
        if (TxtYMax != null)
        {
            TxtYMax.text = string.Format(CultureInfo.InvariantCulture, format, YMax);
        }
        if (TxtYMin != null)
        {
            TxtYMin.text = string.Format(CultureInfo.InvariantCulture, format, YMin);
        }
        RefreshBars();
    }

    public void DisplayValues(float[] values)
    {
        _values = values ?? new float[0];
        RefreshBars();
    }

    private void RefreshBars()
    {
        foreach (var barObj in _valueBars)
        {
            Destroy(barObj);
        }

        _valueBars.Clear();

        if (_values == null || _values.Length == 0)
        {
            return;
        }

        var width = BarWidth == 0.0f ? ChartInner.rect.width / _values.Length : BarWidth;
        foreach (var value in _values)
        {
            var barObj = InstantiateBar(value, width);
            _valueBars.Add(barObj);
        }
    }

    private BarChartItem InstantiateBar(float value, float width)
    {
        var barObj = Instantiate(BarItemPrefab, ChartInner);
        barObj.Parent = this;
        var relativeValue = (value - YMin) / (YMax - YMin);
        relativeValue = Mathf.Clamp01(relativeValue);

        barObj.Value = relativeValue;
        barObj.Width = width;
        barObj.Color = Color.Lerp(MinColor, MaxColor, relativeValue);
        return barObj;
    }
}

