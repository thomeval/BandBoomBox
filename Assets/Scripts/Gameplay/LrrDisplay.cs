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
    private SongManager _songManager;
    private float _transitionTime = 0.5f;
    private void Awake()
    {
        Helpers.AutoAssign(ref _songManager);       
    }

    public void Update()
    {
        if (LrrData == null || _songManager == null)
        {
            return;
        }

        var currentTime = _songManager.GetSongPositionInBeats();

        var currentInterval = (int)Math.Floor(currentTime / LrrData.IntervalSizeBeats);
        var currentIntervalFraction = (currentTime % LrrData.IntervalSizeBeats);
        if (currentIntervalFraction < 0)
        {
            currentIntervalFraction += LrrData.IntervalSizeBeats;
        }
        var transitionStart = LrrData.IntervalSizeBeats - _transitionTime;
        var lerpPoint = Mathf.InverseLerp(transitionStart, LrrData.IntervalSizeBeats, currentIntervalFraction);

        LrrBarChart.XOffset = currentInterval + lerpPoint;
    }

    public void SetFromData(LrrData lrrData)
    {
        var maxNps = lrrData.Intervals.Max();
        LrrData = lrrData;
        LrrBarChart.SetYAxis(0, maxNps);
        LrrBarChart.DisplayValues(lrrData.Intervals);
        var suffix = "P" + (lrrData.PlayerSlot);
        PlayerIdentifierSprite.SetCategoryAndLabel("PlayerIdentifiers", suffix);
    }
}