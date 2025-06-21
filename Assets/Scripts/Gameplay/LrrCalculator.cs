using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LrrCalculator : MonoBehaviour
{
    [SerializeField]
    private float _intervalSizeBeats = 8.0f; // Default LRR size in seconds

    public float IntervalSizeBeats
    {
        get => _intervalSizeBeats;
        set => _intervalSizeBeats = Mathf.Max(value, 0.1f);
    }

    public LrrData CalculateLrrData(List<Note> notes, float songLengthInBeats, float bpm)
    {
        var totalIntervals = (int)Math.Ceiling(songLengthInBeats / _intervalSizeBeats);
        var result = new LrrData
        {
            IntervalSizeBeats = _intervalSizeBeats,
            Intervals = new float[totalIntervals] 
        };
        notes = notes.OrderBy(e => e.Position).ToList();

        int currentInterval = 0;
        float currentIntervalTime = _intervalSizeBeats;
        foreach (var note in notes)
        {
            if (note.Position > currentIntervalTime)
            {
                currentInterval++;
                currentIntervalTime += _intervalSizeBeats;
            }

            result.Intervals[currentInterval]++;
        }

        // Divide by the interval size so that each interval hold the notes per second of that interval.
        for (int x = 0; x < result.Intervals.Length; x++)
        {
            result.Intervals[x] /= _intervalSizeBeats / bpm * 60;
        }

        return result;
    }

    public float CalculateMaxNps(List<Note> notes, float songLengthInBeats, float bpm)
    {
        if (notes == null || notes.Count == 0)
        {
            return 0f;
        }
        var lrrData = CalculateLrrData(notes, songLengthInBeats, bpm);
        return lrrData.Intervals.Max();
    }
}