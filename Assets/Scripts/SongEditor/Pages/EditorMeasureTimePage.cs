using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorMeasureTimePage : EditorPageManager
{
    public override EditorPage EditorPage
    {
        get { return EditorPage.MeasureTime; }
    }

    public InputField TxtCurrentTime;
    public Button DefaultButton;

    public Action<float?> OnMeasureComplete;

    private SongManager _songManager;

    void Awake()
    {
        _songManager = FindObjectOfType<SongManager>();
    }
    void Start()
    {

    }

    void Update()
    {
        TxtCurrentTime.text = string.Format(CultureInfo.InvariantCulture, "{0:F3}", _songManager.GetRawAudioPosition());
    }

    public void BeginMeasure(float startTime)
    {
        EventSystem.current.SetSelectedGameObject(DefaultButton.gameObject);
        _songManager.SongLoaded += (sender, args) =>
        {
            _songManager.StartSong();
            if (startTime < 0.0f)
            {
                startTime = _songManager.GetAudioLength() + startTime;
            }
            _songManager.SetAudioPosition(startTime);
        };
        _songManager.LoadSong(Parent.CurrentSong);

    }

    public override void HandleInput(InputEvent inputEvent)
    {

        base.HandleInput(inputEvent);
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

    public void BtnSet_OnClick()
    {
        var result = _songManager.GetRawAudioPosition();
        result = (float) Math.Round(result, 2);
        _songManager.StopSong();
        OnMeasureComplete(result);
    }

    #endregion
}
