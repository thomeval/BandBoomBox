using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    public static readonly int[] ScrollSpeeds = { 200, 250, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000 };

    public static readonly string[] NoteSkins = {"Default"};

    public static readonly string[] LabelSkins = {"ABXY","BAYX", "Symbols", "WASD", "None"};

    public PlayerHudManager HudManager;
    private InputManager _inputManager;

    void Awake()
    {
        _inputManager = this.GetComponent<InputManager>();
    }

    void Start()
    {
        Reset();
        RefreshHud();
    }

    #region Properties
    
    [SerializeField]
    private int _perfPoints;
    public int PerfPoints
    {
        get { return _perfPoints; }
        set
        {
            _perfPoints = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private int _maxPerfPoints;

    public int MaxPerfPoints
    {
        get { return _maxPerfPoints; }
        set
        {
            _maxPerfPoints = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private int _scrollSpeed;
    public int ScrollSpeed
    {
        get { return _scrollSpeed; }
        set { _scrollSpeed = value; }
    }
    public int Slot;

    [SerializeField]
    private int _combo;

    public int Combo
    {
        get { return _combo; }
        set
        {
            _combo = value;
            this.MaxCombo = Math.Max(this.Combo, this.MaxCombo);
            RefreshHud();
        }
    }

    [SerializeField]
    private int _maxCombo;

    public int MaxCombo
    {
        get { return _maxCombo; }
        set
        {
            _maxCombo = value;
            RefreshHud();
        }
    }

    public string Name;

    public string NameOrPlayerNumber
    {
        get { return this.Name == "Guest" ? "Player " + this.Slot : this.Name; }
    }
    
    [SerializeField]
    private int _ranking;

    public int Ranking
    {
        get { return _ranking; }
        set
        {
            _ranking = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private Difficulty _difficulty;

    public Difficulty Difficulty
    {
        get { return _difficulty; }
        set
        {
            _difficulty = value;
            RefreshHud();
        }
    }

    public float? Goal;

    public long Exp;


    [SerializeField]
    private bool _turboActive;

    public bool TurboActive
    {
        get { return _turboActive; }
        set
        {
            _turboActive = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private string _profileId;

    public string ProfileId
    {
        get { return _profileId; }
        set
        {
            _profileId = value;
        }
    }

    [SerializeField]
    private bool _mistakeSfxEnabled = true;
    public bool MistakeSfxEnabled
    {
        get
        {
            return _mistakeSfxEnabled;
        }
        set
        {
            _mistakeSfxEnabled = value;
        }
    }

    [SerializeField]
    private bool _rumbleEnabled = true;
    public bool RumbleEnabled
    {
        get
        {
            return _rumbleEnabled;
        }
        set
        {
            _rumbleEnabled = value;
        }
    }

    public void RefreshHud()
    {
        if (HudManager != null)
        {
            HudManager.UpdateHud(this);
        }
    }

    public TimingDisplayType TimingDisplayType = TimingDisplayType.Words;

    public string NoteSkin = "Default";
    public string LabelSkin = "None";

    public float PerfPercent
    {
        get { return this.MaxPerfPoints == 0 ? 0 : (1.0f * this.PerfPoints / this.MaxPerfPoints); }
    }

    public int? GoalPerfPoints
    {
        get
        {
            if (Goal == null)
            {
                return null;
            }

            return (int) (MaxPerfPoints * Goal.Value);
        }
        
    }

    public int MaxPerfPointsWithMistakes
    {
        get
        {
            var result = MaxPerfPoints;
            var maxPerHit = HitJudge.JudgePerfPointValues[JudgeResult.Perfect];

            foreach (var hit in EarlyHits.Union(LateHits).ToList())
            {
                var lostPerHit = maxPerHit - HitJudge.JudgePerfPointValues[hit.Key];
                result -= hit.Value * lostPerHit;
            }

            result -= Mistakes[JudgeResult.Miss] * maxPerHit;
            return result;
        }
    }

    [SerializeField]
    private BeatLineType _beatLineType = BeatLineType.Beat;
    public BeatLineType BeatLineType 
    {
        get { return _beatLineType; }
        set { _beatLineType = value; }
    }

    public bool ControllerConnected
    {
        get { return _inputManager.ControllerConnected; }
    }

    public int Level
    {
        get
        {
            return ExpLevelUtils.GetLevel(this.Level);
        }
    }



    public int SongsPlayed;

    public string ChartGroup = "Main";

    public Dictionary<JudgeResult, int> EarlyHits = new();
    public Dictionary<JudgeResult, int> LateHits = new();
    public Dictionary<JudgeResult, int> Mistakes = new();

    public int TotalHits
    {
        get
        {
            return EarlyHits.Sum(e => e.Value) + LateHits.Sum(e => e.Value);
        }
    }

    public int TotalHitsWithMisses
    {
        get
        {
            return EarlyHits.Sum(e => e.Value) + LateHits.Sum(e => e.Value) + Mistakes[JudgeResult.Miss];
        }
    }

    public float HitAccuracyAverage
    {
        get
        {
            if (TotalHits == 0)
            {
                return 0;
            }

            return HitAccuracyTotal / TotalHits;
        }
    }

    public float HitAccuracyTotal { get; set; }

    public float HitDeviationAverage
    {
        get
        {
            if (TotalHits == 0)
            {
                return 0;
            }
            return HitDeviationTotal / TotalHits;
        }
    }
    public float HitDeviationTotal { get; set; }

    #endregion

    public void ApplyHitResult(HitResult result)
    {
        UpdateCombo(result.JudgeResult);
        UpdateHitCounts(result);
        UpdatePerfPoints(result);
        UpdateAccuracyDeviation(result);
        HudManager.DisplayHitResult(result);

        if (result.JudgeResult == JudgeResult.Wrong && RumbleEnabled)
        {
            TriggerRumbleForWrongInput();
        }
    }

    private void UpdateAccuracyDeviation(HitResult result)
    {
        this.HitAccuracyTotal += Math.Abs(result.Deviation);
        this.HitDeviationTotal += result.Deviation;
    }

    private void UpdatePerfPoints(HitResult result)
    {
        this.PerfPoints += result.PerfPoints;
    }

    private void UpdateHitCounts(HitResult result)
    {
        if (result.JudgeResult == JudgeResult.Wrong || result.JudgeResult == JudgeResult.Miss)
        {
            Mistakes[result.JudgeResult]++;
        }
        else switch (result.DeviationResult)
        {
            case DeviationResult.Early:
                EarlyHits[result.JudgeResult]++;
                break;
            case DeviationResult.Late:
                LateHits[result.JudgeResult]++;
                break;
        }
    }

    private void UpdateCombo(JudgeResult result)
    {
        var comboBreak = HitJudge.IsComboBreak(result);
        if (!comboBreak.HasValue)
        {
            return;
        }

        if (comboBreak.Value)
        {
            this.Combo = 0;
        }
        else
        {
            this.Combo++;
        }

    }

    public void AutoSetLabelSkin()
    {
        _inputManager ??= this.GetComponent<InputManager>();
        var labelSkin = _inputManager.GetPreferredControllerType();
        if (labelSkin != null)
        {
            this.LabelSkin = labelSkin;
        }
    }

    public void Reset()
    {
        EarlyHits = new Dictionary<JudgeResult, int>();
        EarlyHits.Add(JudgeResult.Crit, 0);
        EarlyHits.Add(JudgeResult.Perfect, 0);
        EarlyHits.Add(JudgeResult.Cool, 0);
        EarlyHits.Add(JudgeResult.Ok, 0);
        EarlyHits.Add(JudgeResult.Bad, 0);

        LateHits = new Dictionary<JudgeResult, int>();
        LateHits.Add(JudgeResult.Crit, 0);
        LateHits.Add(JudgeResult.Perfect, 0);
        LateHits.Add(JudgeResult.Cool, 0);
        LateHits.Add(JudgeResult.Ok, 0);
        LateHits.Add(JudgeResult.Bad, 0);

        Mistakes = new Dictionary<JudgeResult, int>();
        Mistakes.Add(JudgeResult.Wrong, 0);
        Mistakes.Add(JudgeResult.Miss, 0);

        this.PerfPoints = 0;
        this.Combo = 0;
        this.MaxCombo = 0;
        this.HitAccuracyTotal = 0.0f;
        this.HitDeviationTotal = 0.0f;
        this.Ranking = 1;
    }

    public int GetBaseExpGain()
    {
        float result = PerfPoints;
        return (int) result;
    }

    public Grade? GetGoalGrade()
    {
        if (Goal == null)
        {
            return null;
        }
        return Helpers.PercentToGrade(Goal.Value);
    }
    public Grade GetCurrentGrade()
    {
        return Helpers.PercentToGrade(PerfPercent);
    }

    public string GetPlayerIdSprite()
    {
        return Slot > 0 ? "P" + Slot : "None";
    }

    public void ApplyExpGain(float modifier)
    {
        this.Exp += (int) (GetBaseExpGain() * modifier);
    }

    #region Helpers

    public void ChangeDifficulty(int delta)
    {
        var newDifficulty = Helpers.EnumAdd(this.Difficulty, delta, true);
        this.Difficulty = newDifficulty;
    }

    public void ChangeScrollSpeed(int delta)
    {
        var newValue = Helpers.GetNextValue(ScrollSpeeds, this.ScrollSpeed, delta, true);
        this.ScrollSpeed = newValue;
    }

    public void ChangeNoteSkin(int delta)
    {
        var newValue = Helpers.GetNextValue(NoteSkins, this.NoteSkin, delta, true);
        this.NoteSkin = newValue;
    }

    public void ChangeLabelSkin(int delta)
    {
        var newValue = Helpers.GetNextValue(LabelSkins, this.LabelSkin, delta, true);
        this.LabelSkin = newValue;
    }

    public void ChangeTimingDisplayType(int delta)
    {
        var newDisplayType = Helpers.EnumAdd(this.TimingDisplayType, delta, true);
        this.TimingDisplayType = newDisplayType;
    }

    public void ToggleTurbo()
    {
        TurboActive = !TurboActive;
    }
    #endregion

    public float HitPercentage(JudgeResult judgeResult)
    {
        var totalHits = TotalHitsWithMisses;

        if (Mistakes.ContainsKey(judgeResult))
        {
            return 1.0f * Mistakes[judgeResult] / totalHits;
        }

        return 1.0f * (EarlyHits[judgeResult] + LateHits[judgeResult]) / totalHits;
    }

    public PlayerScore GetPlayerScore(string songId, int songVersion)
    {
        var result = new PlayerScore
        {
            Difficulty = this.Difficulty,
            PerfPoints = this.PerfPoints,
            MaxPerfPoints = this.MaxPerfPoints,
            SongId = songId,
            SongVersion = songVersion,
            DateTime = DateTime.Now,
            ChartGroup = this.ChartGroup,
            MaxCombo = this.MaxCombo
        };
        return result;
    }

    public void ApplyInputActions(string json)
    {
        /*
        var playerInput = GetComponent<PlayerInput>();
        //  asset.devices = playerInput.devices;
        playerInput.actions.LoadFromJson(json);
        */
    }

    public ProfileData GetProfileData()
    {
        return new ProfileData
        {
            ID = this.ProfileId,
            Name = this.Name,
            ScrollSpeed = this.ScrollSpeed,
            Exp = this.Exp,
            TimingDisplayType = this.TimingDisplayType,
            Goal = this.Goal,
            Difficulty = this.Difficulty,
            SongsPlayed = this.SongsPlayed,
            MistakeSfxEnabled = this.MistakeSfxEnabled
        };
    }

    public FullComboType GetFullComboType()
    {
        var totalNotes = this.EarlyHits.Sum(e => e.Value) 
                         + this.LateHits.Sum(e => e.Value)
                         + this.Mistakes[JudgeResult.Miss];     // Include misses, but not wrongs.

        var perfectHits = this.EarlyHits[JudgeResult.Perfect] + this.LateHits[JudgeResult.Perfect]
                                                              + this.EarlyHits[JudgeResult.Crit] +
                                                              this.LateHits[JudgeResult.Crit];

        Debug.Log($"Total Notes: {totalNotes}\r\n" +
                  $"Perfect Hits: {perfectHits}\r\n" +
                  $"Max Combo: {this.MaxCombo}");
        
        if (perfectHits == totalNotes)
        {
            return FullComboType.PerfectFullCombo;
        }
        
        if (this.MaxCombo == totalNotes)
        {
            return FullComboType.FullCombo;
        }

        if (this.MaxCombo >= totalNotes / 2)
        {
            return FullComboType.SemiFullCombo;
        }

        return FullComboType.None;
    }

    public void TriggerRumbleForCritHit()
    {
        _inputManager.TriggerRumble(0.25f, 0.4f);
    }

    public void TriggerRumbleForTitleStart()
    {
        _inputManager.TriggerRumble(0.5f, 0.5f);
    }

    public void TriggerRumbleForWrongInput()
    {
        _inputManager.TriggerRumble(0.65f, 0.2f);
    }
}
