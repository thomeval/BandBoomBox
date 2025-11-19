using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public Text TxtScore;
    public Text TxtBpm;
    public Text TxtDebug;
    public Text TxtTeamCombo;
    public Text TxtSongTime;
    public Text TxtSongCurrentBeat;
    public Text TxtSongCurrentSection;
    public Text TxtSongTitle;
    public Text TxtMxGainRate;
    public MxMeter MxMeter;
    public EnergyMeter EnergyMeter;
    public StarMeter StarMeter;
    public GameplayStateValues StateValues;
    public SongStarScoreValues SongStarScoreValues = new();

    public List<PlayerHudManager> PlayerHudManagers;
    public bool UpdateStarsWithScore = true;

    private SongManager _songManager;

    private string _songTitleText;
    public string SongTitleText
    {
        get { return _songTitleText; }
        set
        {
            _songTitleText = value;
            TxtSongTitle.text = _songTitleText;
        }
    }

    public double Stars
    {
        get { return StarMeter.Value; }
        set { StarMeter.Value = value; }
    }

    private long _displayedScore = 0;

    void Awake()
    {
        _songManager = FindObjectOfType<SongManager>();
    }

    void FixedUpdate()
    {
        UpdateScore();
    }

    void Update()
    {
        MxMeter.Multiplier = (float)StateValues.Multiplier;

        TxtTeamCombo.text = string.Format(CultureInfo.InvariantCulture, "{0:000}", StateValues.TeamCombo);
        var mxGainBonus = StateValues.MxGainRate - 1.0f;

        TxtMxGainRate.text = string.Format(CultureInfo.InvariantCulture, "+{0:N0}%", mxGainBonus * 100);
        TxtMxGainRate.enabled = mxGainBonus > 0.0f;

        if (_songManager.CurrentSong == null)
        {
            return;
        }
        TxtBpm.text = string.Format(CultureInfo.InvariantCulture, "{0:N1}", _songManager.CurrentSong.Bpm);
        var songLength = (int)(_songManager.GetPlayableLength() - _songManager.GetSongPosition());
        songLength = Math.Max(songLength, 0);
        TxtSongTime.text = string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}", songLength / 60, songLength % 60);
        TxtSongCurrentSection.text = _songManager.GetCurrentSectionName();

        DisplayBeat(_songManager.GetSongPositionInBeats());

    }

    private void DisplayBeat(float songPositionInBeats)
    {
        TxtSongCurrentBeat.text = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", _songManager.GetSongPositionInBeats());
        foreach (var phud in PlayerHudManagers.Where(e => e.isActiveAndEnabled))
        {
            phud.DisplayBeat(songPositionInBeats);
        }
    }

    private void UpdateScore()
    {
        var diff = StateValues.Score - _displayedScore;

        if (diff <= 3)
        {
            _displayedScore = StateValues.Score;
        }
        else
        {
            _displayedScore += diff / 3;
        }
        TxtScore.text = string.Format(CultureInfo.InvariantCulture, "{0:00000000}", _displayedScore);

        if (UpdateStarsWithScore)
        {
            var stars = SongStarScoreValues.GetStarFraction(_displayedScore);
            StarMeter.Value = stars;
        }
    }

    public void UpdateEnergyMeter(bool turboActive)
    {
        EnergyMeter.Energy = StateValues.Energy;
        EnergyMeter.SetMaxEnergy(StateValues.MaxEnergy);
        EnergyMeter.TurboActive = turboActive;
    }

    public PlayerHudManager GetPlayerHudManager(int playerSlot)
    {
        return PlayerHudManagers.SingleOrDefault(e => e.Slot == playerSlot);
    }
}
