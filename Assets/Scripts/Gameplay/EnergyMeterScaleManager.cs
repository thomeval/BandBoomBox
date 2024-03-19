using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMeterScaleManager : MonoBehaviour
{
    public List<GameObject> Children;
    public GameObject[] LinePrefabs;

    // These values are multiplied by 100 compared to EnergyMeter to avoid floating point errors.
    public int[] ScaleValues = { 50, 100, 400 };

    private EnergyMeter _parent;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _parent = GetComponentInParent<EnergyMeter>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void DrawScaleLines()
    {
        this.Clear();

        var maxEnergy = _parent.MaxEnergy * 100;
        var tickAmount = ScaleValues[0];
        var meterHeight = _rectTransform.rect.height;

        for (int currentPos = tickAmount; currentPos < maxEnergy; currentPos += tickAmount)
        {
            var selectedPrefab = GetPrefabForTick(currentPos);
            var line = Instantiate(selectedPrefab, transform);
            var yPos = (currentPos / maxEnergy) * meterHeight;
            yPos -= meterHeight / 2;
            line.transform.localPosition = new Vector3(0, (float)yPos, 0);
            Children.Add(line);
        }
    }

    private GameObject GetPrefabForTick(int tick)
    {
        for (int x = ScaleValues.Length - 1; x >= 0; x--)
        {
            if (tick % ScaleValues[x] == 0)
            {
                return LinePrefabs[x];
            }
        }
        throw new InvalidOperationException();
    }

    public void Clear()
    {
        this.gameObject.ClearChildren();
        this.Children.Clear();
    }
}
