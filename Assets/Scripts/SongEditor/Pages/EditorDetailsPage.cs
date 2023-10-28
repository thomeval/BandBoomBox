using System;
using System.Globalization;
using UnityEngine.UI;
// ReSharper disable CompareOfFloatsByEqualityOperator

public class EditorDetailsPage : EditorPageManager
{
    public override EditorPage EditorPage
    {
        get { return EditorPage.Details; }
    }

    public InputField TxtTitle;
    public InputField TxtSubtitle;
    public InputField TxtArtist;
    public InputField TxtChartAuthor;
    public InputField TxtBpm;
    public InputField TxtAudioStart;
    public InputField TxtOffset;
    public InputField TxtLength;
    public InputField TxtBeatsInMeasure;
    public InputField TxtIssues;
    public InputField TxtVersion;
    public InputField TxtUrl;

    public Text TxtErrorMessage;

    public Button BtnBack;
    public Button BtnNext;

    public Toggle ChkAutoAudioStart;

    private bool _isValid;

    public SongData CurrentSong
    {
        get { return Parent.CurrentSong; }
    }

    private void DisplaySongData()
    {
        if (CurrentSong == null)
        {
            return;
        }

        TxtTitle.text = CurrentSong.Title;
        TxtSubtitle.text = CurrentSong.Subtitle;
        TxtArtist.text = CurrentSong.Artist;
        TxtChartAuthor.text = CurrentSong.ChartAuthor;
        TxtIssues.text = CurrentSong.Issues;
        TxtVersion.text = "" + CurrentSong.Version;
        TxtUrl.text = CurrentSong.Url;

        TxtAudioStart.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}", CurrentSong.AudioStart);
        TxtBpm.text = string.Format(CultureInfo.InvariantCulture, "{0:F1}", CurrentSong.Bpm);
        TxtOffset.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}", CurrentSong.Offset);
        TxtLength.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}", CurrentSong.Length);
        TxtBeatsInMeasure.text = string.Format(CultureInfo.InvariantCulture, "{0:F0}", CurrentSong.BeatsPerMeasure);
    }

    public bool ApplySongData()
    {
        _isValid = true;
        TxtErrorMessage.text = "";
        CurrentSong.Title = TxtTitle.text;
        CurrentSong.Subtitle = TxtSubtitle.text;
        CurrentSong.Artist = TxtArtist.text;
        CurrentSong.ChartAuthor = TxtChartAuthor.text;
        CurrentSong.Bpm = GetNumberValueOrDefault(TxtBpm);
        CurrentSong.Offset = GetNumberValueOrDefault(TxtOffset);
        CurrentSong.AudioStart = GetNumberValueOrDefault(TxtAudioStart);
        CurrentSong.Length = GetNumberValueOrDefault(TxtLength);
        CurrentSong.Issues = TxtIssues.text;
        CurrentSong.Url = TxtUrl.text;

        Validate(TxtAudioStart, "Audio Start", out CurrentSong.AudioStart);
        Validate(TxtBpm, "BPM", out CurrentSong.Bpm);
        Validate(TxtOffset, "Offset", out CurrentSong.Offset);
        Validate(TxtLength, "Length", out CurrentSong.Length);
        Validate(TxtBeatsInMeasure, "Beats In Measure", out CurrentSong.BeatsPerMeasure);
        Validate(TxtVersion, "Version", out CurrentSong.Version);

        return _isValid;
    }

    private float GetNumberValueOrDefault(InputField field)
    {
        float.TryParse(field.text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
        return result;
    }

    private void Validate(InputField inputField, string description, out int destination)
    {

        if (!int.TryParse(inputField.text, NumberStyles.Any, CultureInfo.InvariantCulture, out destination))
        {
            _isValid = false;
            TxtErrorMessage.text += $"{description} is not a valid number.\r\n";
        }
    }

    private void Validate(InputField inputField, string description, out float destination)
    {
        if (!float.TryParse(inputField.text, NumberStyles.Any, CultureInfo.InvariantCulture, out destination))
        {
            _isValid = false;
            TxtErrorMessage.text += $"{description} is not a valid number.\r\n";
        }
    }

    void Awake()
    {
        BtnBack.onClick.AddListener(BtnBack_OnClick);
        BtnNext.onClick.AddListener(BtnNext_OnClick);
    }

    void OnEnable()
    {
        DisplaySongData();
    }

    void BtnBack_OnClick()
    {
        if (Parent.IsExistingSong)
        {
            Parent.CurrentPage = EditorPage.MainMenu;
        }
        else
        {
            Parent.CurrentPage = EditorPage.Basics;
        }
    }

    void BtnNext_OnClick()
    {
        if (!ApplySongData())
        {
            return;
        }

        var validateResult = SongValidator.Validate(CurrentSong);

        if (validateResult != "")
        {
            TxtErrorMessage.text = validateResult;
            return;
        }

        Parent.SaveCurrentSong(false);
        Parent.CurrentPage = EditorPage.ChartList;
    }

    public void BtnMeasureBpm_OnClick()
    {
        ApplySongData();
        Parent.RequestMeasureBpm((result) =>
        {
            ApplyMeasurement(result, ref CurrentSong.Bpm);
            DisplaySongData();
        }, CurrentSong.Offset);
    }

    public void BtnMeasureOffset_OnClick()
    {
        ApplySongData();
        Parent.RequestMeasureTime((result) =>
        {
            ApplyMeasurement(result, ref CurrentSong.Offset);
            AutoSetAudioStart();
            DisplaySongData();
        });
    }

    public void BtnMeasureAudioStart_OnClick()
    {
        ApplySongData();
        Parent.RequestMeasureTime((result) =>
        {
            ApplyMeasurement(result, ref CurrentSong.AudioStart);
            DisplaySongData();
        });
    }
    public void BtnMeasureLength_OnClick()
    {
        ApplySongData();
        Parent.RequestMeasureTime((result) =>
        {
            ApplyMeasurement(result, ref CurrentSong.Length);
            DisplaySongData();
        }, -30.0f);
    }

    public void BtnFineTune_OnClick()
    {
        Parent.RequestFineTune(() =>
        {
            Parent.CurrentPage = this.EditorPage;
            DisplaySongData();
        });
    }
    public void TxtOffset_OnValueChanged()
    {
        CurrentSong.Offset = GetNumberValueOrDefault(TxtOffset);
        AutoSetAudioStart();
    }

    public void ChkAutoAudioStart_OnValueChanged()
    {
        AutoSetAudioStart();
    }

    private void ApplyMeasurement(float? result, ref float target)
    {
        Parent.CurrentPage = this.EditorPage;
        if (result == null)
        {
            return;
        }

        target = result.Value;
    }

    private void AutoSetAudioStart()
    {

        if (CurrentSong == null)
        {
            return;
        }
        if (CurrentSong.Bpm == 0.0f)
        {
            return;
        }

        if (!ChkAutoAudioStart.isOn)
        {
            return;
        }

        var fourMeasures = 60 / CurrentSong.Bpm * 16;
        CurrentSong.AudioStart = Math.Max(0.0f, CurrentSong.Offset - fourMeasures);
        TxtAudioStart.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}", CurrentSong.AudioStart);
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.Back:
                BtnBack_OnClick();
                break;
        }
    }
}
