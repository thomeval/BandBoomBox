using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public GameplayManager GameplayManager;

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

    public List<PlayerHudManager> PlayerHudManagers;

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

    private long _displayedScore = 0;

    void Awake()
    {
        _songManager = FindObjectOfType<SongManager>();
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        UpdateScore();
    }
    void Update()
    {
        var beat = _songManager.GetSongPositionInBeats();

        MxMeter.BeatFraction = beat - (int) beat;
        MxMeter.Multiplier = (float) GameplayManager.Multiplier;
    
        TxtTeamCombo.text = string.Format("{0:000}",GameplayManager.TeamCombo);
        var mxGainBonus = GameplayManager.MxGainRate - 1.0f;

        TxtMxGainRate.text = string.Format("+{0:P0}", mxGainBonus);
        TxtMxGainRate.enabled = mxGainBonus> 0.0f;
        
        if (_songManager.CurrentSong == null)
        {
            return;
        }
        TxtBpm.text = string.Format(CultureInfo.InvariantCulture, "{0:N1}", _songManager.CurrentSong.Bpm);   
        var songLength = (int) (_songManager.GetPlayableLength() - _songManager.GetSongPosition());
        songLength = Math.Max(songLength, 0);
        TxtSongTime.text = string.Format("{0}:{1:00}", songLength / 60, songLength % 60);
        TxtSongCurrentSection.text = _songManager.GetCurrentSection();
        
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
        var diff = GameplayManager.Score - _displayedScore;

        if (diff <= 3)
        {
            _displayedScore = GameplayManager.Score;
        }
        else
        {
            _displayedScore += diff / 3;
        }
        TxtScore.text = string.Format("{0:00000000}", _displayedScore);

        var stars = GameplayManager.GetStarFraction(_displayedScore);
        StarMeter.Value = stars;
    }

    public void UpdateEnergy(float energy, bool turboActive)
    {
        EnergyMeter.Energy = energy;
        EnergyMeter.TurboActive = turboActive;
    }

    public PlayerHudManager GetPlayerHudManager(int playerSlot)
    {
        return PlayerHudManagers.SingleOrDefault(e => e.Slot == playerSlot);
    }
}
