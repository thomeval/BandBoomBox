using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorMeasureBpmPage : EditorPageManager
{
    public override EditorPage EditorPage
    {
        get { return EditorPage.MeasureBpm; }
    }

    public InputField TxtNumberOfHits;
    public InputField TxtEstimatedBpm;
    public InputField TxtLastBeat;
    public InputField TxtLast10;
    public InputField TxtLast32;
    public Button DefaultButton;

    public Action<float?> OnMeasureComplete;
    
    private SongManager _songManager;
    private DateTime? _lastHitTime;

    [SerializeField]
    private readonly List<double> _beats = new List<double>();

    void Awake()
    {
        _songManager = FindObjectOfType<SongManager>();
    }

    public void BeginMeasure(float startTime)
    {
        EventSystem.current.SetSelectedGameObject(DefaultButton.gameObject);
        _lastHitTime = null;
        _beats.Clear();
        CalculateEstimates();
        _songManager.LoadSong(Parent.CurrentSong, () => OnSongLoaded(startTime));
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        base.HandleInput(inputEvent);
    }

    private void OnSongLoaded(float startTime)
    {
        {
            _songManager.StartSong();
            if (startTime < 0.0f)
            {
                startTime = _songManager.GetAudioLength() + startTime;
            }

            _songManager.SetAudioPosition(startTime);
        };
    }
    private void CalculateEstimates()
    {

        TxtLastBeat.text = RollingAverage(1);
        TxtLast10.text = RollingAverage(10);
        TxtLast32.text = RollingAverage(32);

        if (_beats.Any())
        {
            TxtEstimatedBpm.text = "" + Math.Round(_beats.Average());
        }
        else
        {
            TxtEstimatedBpm.text = "---";
        }

        TxtNumberOfHits.text  = "" +_beats.Count();
    }

    private string RollingAverage(int count)
    {
        if (_beats.Count < count)
        {
            return "---";
        }

        return string.Format(CultureInfo.InvariantCulture, "{0:F1}", _beats.Take(count).Average());
    }

    private void AddHit()
    {
        if (_lastHitTime == null)
        {
            _lastHitTime = DateTime.Now;
            return;
        }

        var diff = (DateTime.Now - _lastHitTime).Value.TotalSeconds;
        var bpm = 60 / diff;
        _beats.Insert(0,(float) bpm);
        _lastHitTime = DateTime.Now;
        
    }

    #region Button Event Handlers

    public void BtnRewind3_OnClick()
    {
        _songManager.SetAudioPosition(_songManager.GetRawAudioPosition() - 3.0f);
    }

    public void BtnRewind30_OnClick()
    {
        _songManager.SetAudioPosition(_songManager.GetRawAudioPosition() - 30.0f);
    }

    public void BtnRewindAll_OnClick()
    {
        _songManager.SetAudioPosition(0.0f);
    }

    public void BtnForward3_OnClick()
    {
        _songManager.SetAudioPosition(_songManager.GetRawAudioPosition() + 3.0f);
    }

    public void BtnForward30_OnClick()
    {
        _songManager.SetAudioPosition(_songManager.GetRawAudioPosition() + 30.0f);
    }

    public void BtnCancel_OnClick()
    {
        _songManager.StopSong();
        OnMeasureComplete(null);
    }

    public void BtnDone_OnClick()
    {

        float? result = null;

        if (_beats.Any())
        {
            result = (float) Math.Round(_beats.Average());
        }
        _songManager.StopSong();
        OnMeasureComplete(result);
    }

    public void BtnReset_OnClick()
    {
        _beats.Clear();
        _lastHitTime = null;
        CalculateEstimates();
    }

    public void BtnBeat_OnClick()
    {
        AddHit();
        CalculateEstimates();
    }

    #endregion
}
