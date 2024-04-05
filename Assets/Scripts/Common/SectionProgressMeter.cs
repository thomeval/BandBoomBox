using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SectionProgressMeter : MonoBehaviour
{
    public SpriteMeter Meter;
    public Text TxtCurrentSection;

    private List<double> _orderedSections = new();
    [SerializeField] private double _currentPosition;

    public double CurrentPosition
    {
        get { return _currentPosition; }
        set
        {
            _currentPosition = value;
            UpdateMeter();
        }
    }

    [SerializeField] private SongData _displayedSong;
    public SongData DisplayedSong
    {
        get
        {
            return _displayedSong;
        }
        set
        {
            _displayedSong = value;
            _orderedSections = DisplayedSong.Sections.Keys.OrderBy(e => e).ToList();

            if (!_orderedSections.Contains(0.0))
            {
                _orderedSections.Add(0.0);
            }
            _orderedSections.Add(DisplayedSong.LengthInBeats);
        }
    }

    public void UpdateMeter()
    {
        if (!_orderedSections.Any())
        {
            return;
        }

        var currentSection = _displayedSong.GetSectionName(CurrentPosition);

        if (CurrentPosition < 0.0)
        {
            currentSection = "Countdown";
        }

        TxtCurrentSection.text = currentSection;

        var startPosition = GetStartPosition(CurrentPosition);
        var endPosition = GetEndPosition(CurrentPosition);

        var value = 0.0;
        if (startPosition != endPosition)
        {
            value = (CurrentPosition - startPosition) / (endPosition - startPosition);

        }

        value = Math.Clamp(value, 0, 1);
        Meter.Value = (float)value;
    }

    private double GetStartPosition(double currentPosition)
    {
        var section = _orderedSections.LastOrDefault(e => e <= currentPosition);
        return section;
    }

    private double GetEndPosition(double currentPosition)
    {
        var section = _orderedSections.FirstOrDefault(e => e > currentPosition);
        return section;
    }
}
