using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarChart : MonoBehaviour
{

    public float YMin = 0.0f;
    public float YMax = 100.0f;
    public Text TxtYMin;
    public Text TxtYMax;
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
        TxtYMax.text = YMax.ToString();
        TxtYMin.text = YMin.ToString();
    }

    public void SetYAxis(float min, float max)
    {
        YMin = min;
        YMax = max;
        TxtYMin.text = YMin.ToString();
        TxtYMax.text = YMax.ToString();
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

        var width = ChartInner.rect.width / _values.Length;
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

