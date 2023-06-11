
using System;
using UnityEngine;

public class SongProgressMeter : MonoBehaviour
{
    public SpriteMeter Meter;

    [SerializeField] private double _currentPosition;
    [SerializeField] private double _startPosition;
    [SerializeField] private double _endPosition;

    public double CurrentPosition
    {
        get { return _currentPosition; }
        set
        {
            _currentPosition = value;
            UpdateMeter();
        }
    }

    public double StartPosition
    {
        get { return _startPosition; }
        set
        {
            _startPosition = value;
            UpdateMeter();
        }
    }

    public double EndPosition
    {
        get { return _endPosition; }
        set
        {
            _endPosition = value;
            UpdateMeter();
        }
    }

    private void UpdateMeter()
    {
        var value = 0.0;
        if (EndPosition - StartPosition != 0.0)
        {
            value = (CurrentPosition - StartPosition) / (EndPosition - StartPosition);
        }

        value = Math.Clamp(value, 0, 1);
        Meter.Value = (float) value;
    }
}
