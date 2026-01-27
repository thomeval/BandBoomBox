using System;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class LrrDisplay : MonoBehaviour
{
    public LrrData LrrData;
    public BarChart LrrBarChart;
    public SpriteResolver PlayerIdentifierSprite;
    public int XOffset = 0;
    public bool UseSongManager = true;
    private SongManager _songManager;
    private float _transitionTime = 0.5f;
    private float _lastTime = float.MinValue;

    private void Awake()
    {
        Helpers.AutoAssign(ref _songManager);
    }

    public void Update()
    {
        if (!UseSongManager || LrrData == null || _songManager == null)
        {
            return;
        }

        var currentTime = _songManager.GetSongPositionInBeats();

        SetCurrentTime(currentTime);
    }

    public void SetCurrentTime(float currentTime)
    {
        if (Math.Abs(currentTime - _lastTime) < 0.001f)
        {
            return;
        }

        var currentInterval = (int)Math.Floor(currentTime / LrrData.IntervalSizeBeats);
        var currentIntervalFraction = (currentTime % LrrData.IntervalSizeBeats);
        if (currentIntervalFraction < 0)
        {
            currentIntervalFraction += LrrData.IntervalSizeBeats;
        }
        var transitionStart = LrrData.IntervalSizeBeats - _transitionTime;
        var lerpPoint = Mathf.InverseLerp(transitionStart, LrrData.IntervalSizeBeats, currentIntervalFraction);

        LrrBarChart.XOffset = currentInterval + lerpPoint + XOffset;
        _lastTime = currentTime;
    }

    public void SetFromData(LrrData lrrData, int playerSlot)
    {
        var maxNps = lrrData.Intervals.Max();
        LrrData = lrrData;
        LrrBarChart.SetYAxis(0, maxNps);
        LrrBarChart.DisplayValues(lrrData.Intervals.ToArray());
        var suffix = "P" + (playerSlot);
        PlayerIdentifierSprite.SetCategoryAndLabel("PlayerIdentifiers", suffix);
    }
}