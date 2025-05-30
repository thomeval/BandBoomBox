using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public const int TICKS_FOR_FIRST_ALLY_BOOST = 200;
    public const int TICKS_INC_PER_BOOST = 100;

    public static readonly int[] ScrollSpeeds = { 200, 250, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000 };

    public static readonly string[] NoteSkins = { "Default" };

    public static readonly string[] LabelSkins = { "ABXY", "BAYX", "Symbols", "WASD", "None" };

    public PlayerHudManager HudManager;
    private InputManager _inputManager;

    public PlayerState PlayerState;

    public ProfileData ProfileData = new() { Name = "Guest" };

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
    private ulong _netId;
    public ulong NetId
    {
        get { return _netId; }
        set
        {
            _netId = value;
        }
    }

    public string DisplayNetId
    {
        get
        {
            if (NetId == PlayerManager.DEFAULT_NET_ID)
            {
                return "-";
            }

            return Helpers.NumberToNetIdLetter(NetId);
        }
    }

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

    public int GetCurrentScrollSpeed(double multiplier)
    {
        var result = ScrollSpeed + ((multiplier - 1) * Momentum);
        return (int)result;
    }

    [field: SerializeField]
    public bool IsLocalPlayer { get; set; }

    public int Slot;

    /// <summary>
    /// For local players, gets this player's slot (1 to 4, with 1 being P1). For remote players, always returns 0.
    /// </summary>
    public int LocalSlot
    {
        get
        {
            if (!IsLocalPlayer)
            {
                return 0;
            }
            return Slot;
        }
    }

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

    [SerializeField]
    private FullComboType _netFullComboType;

    /// <summary>
    /// For remote players, gets or sets the full combo type that this player has achieved. Not used for local players.
    /// </summary>
    public FullComboType NetFullComboType
    {
        get { return _netFullComboType; }
        set
        {
            _netFullComboType = value;
            RefreshHud();
        }
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
    private int _allyBoosts;
    public int AllyBoosts
    {
        get { return _allyBoosts; }
        set
        {
            _allyBoosts = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private int _allyBoostTicks;
    public int AllyBoostTicks
    {
        get { return _allyBoostTicks; }
        set
        {
            _allyBoostTicks = Math.Max(0, value);
            while (_allyBoostTicks >= TicksForNextBoost)
            {
                _allyBoostTicks -= TicksForNextBoost;
                TicksForNextBoost += TICKS_INC_PER_BOOST;
                _allyBoosts++;
            }
            RefreshHud();
        }
    }


    [SerializeField]
    private int _ticksForNextBoost;
    public int TicksForNextBoost
    {
        get { return _ticksForNextBoost; }
        set
        {
            _ticksForNextBoost = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private int _allyBoostsReceived;
    public int AllyBoostsReceived
    {
        get { return _allyBoostsReceived; }
        set
        {
            _allyBoostsReceived = value;
            RefreshHud();
        }
    }

    [SerializeField]
    private int _allyBoostsProvided;
    public int AllyBoostsProvided
    {
        get { return _allyBoostsProvided; }
        set
        {
            _allyBoostsProvided = value;
            RefreshHud();
        }
    }


    public bool IsParticipating;

    #region Profile Data Properties

    public string ProfileId
    {
        get
        {
            return ProfileData.ID;
        }
    }

    public string Name
    {
        get { return ProfileData.Name; }
        set { ProfileData.Name = value; }
    }

    public string NameOrPlayerNumber
    {
        get { return this.ProfileData.Name == "Guest" ? "Player " + this.Slot : this.ProfileData.Name; }
    }

    public bool MistakeSfxEnabled
    {
        get { return ProfileData.MistakeSfxEnabled; }
        set { ProfileData.MistakeSfxEnabled = value; }
    }

    public bool RumbleEnabled
    {
        get { return ProfileData.RumbleEnabled; }
        set { ProfileData.RumbleEnabled = value; }
    }

    public int Momentum
    {
        get { return ProfileData.Momentum; }
        set
        {
            ProfileData.Momentum = value;
            RefreshHud();
        }
    }

    public float? Goal
    {
        get { return ProfileData.Goal; }
        set
        {
            ProfileData.Goal = value;
            RefreshHud();
        }
    }

    public long Exp
    {
        get { return ProfileData.Exp; }
        set
        {
            ProfileData.Exp = value;
            RefreshHud();
        }
    }

    public int ScrollSpeed
    {
        get { return ProfileData.ScrollSpeed; }
        set { ProfileData.ScrollSpeed = value; }
    }

    public TimingDisplayType TimingDisplayType
    {
        get { return ProfileData.TimingDisplayType; }
        set { ProfileData.TimingDisplayType = value; }
    }

    public DateTime LastPlayed
    {
        get { return ProfileData.LastPlayed; }
        set { ProfileData.LastPlayed = value; }
    }

    public bool CanReceiveAllyBoosts
    {
        get { return ProfileData.AllyBoostMode == AllyBoostMode.On || ProfileData.AllyBoostMode == AllyBoostMode.ReceiveOnly; }
    }

    public bool CanProvideAllyBoosts
    {
        get { return ProfileData.AllyBoostMode == AllyBoostMode.On || ProfileData.AllyBoostMode == AllyBoostMode.ProvideOnly; }
    }


    #endregion


    public void RefreshHud()
    {
        if (HudManager != null)
        {
            HudManager.UpdateHud(this);
        }
    }

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

            return (int)(MaxPerfPoints * Goal.Value);
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

    public string ControllerType
    {
        get { return _inputManager?.ControllerType ?? "None"; }
    }

    public bool ControllerConnected
    {
        get { return _inputManager.ControllerConnected; }
    }

    public const string DEFAULT_CHART_GROUP = "Main";
    public string ChartGroup = DEFAULT_CHART_GROUP;

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

    public string GroupAndDifficulty
    {
        get
        {
            if (ChartGroup == DEFAULT_CHART_GROUP)
            {
                return Helpers.GetDisplayName(this.Difficulty);
            }
            return ChartGroup + ": " + Helpers.GetDisplayName(this.Difficulty);
        }
    }
    #endregion

    public void ApplyHitResult(HitResult result)
    {
        HudManager.DisplayHitResult(result);
        ApplyBoost(result);

        UpdateCombo(result.JudgeResult);
        UpdateHitCounts(result);
        UpdatePerfPoints(result);
        UpdateAccuracyDeviation(result);

        if (result.JudgeResult == JudgeResult.Wrong && RumbleEnabled)
        {
            TriggerRumbleForWrongInput();
        }
    }

    private void ApplyBoost(HitResult result)
    {
        if (this.CanProvideAllyBoosts)
        {
            this.AllyBoostTicks += HitJudge.JudgeAllyBoostTickValues[result.JudgeResult];
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
        var effectiveResult = result.JudgeResult == JudgeResult.CoolWithBoost ? JudgeResult.Perfect : result.JudgeResult;

        if (effectiveResult == JudgeResult.Wrong || effectiveResult == JudgeResult.Miss)
        {
            Mistakes[result.JudgeResult]++;
            return;
        }

        switch (result.DeviationResult)
        {
            case DeviationResult.Early:
                EarlyHits[effectiveResult]++;
                break;
            case DeviationResult.Late:
                LateHits[effectiveResult]++;
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

    public void AutoSetLabelSkin(bool fromController)
    {
        _inputManager ??= this.GetComponent<InputManager>();

        if (fromController)
        {
            AutoSetLabelSkinFromController();
        }
        else
        {
            AutoSetLabelSkinFromProfile();
        }
    }

    private void AutoSetLabelSkinFromController()
    {
        var labelSkin = _inputManager.GetPreferredNoteLabels();
        this.LabelSkin = labelSkin ?? LabelSkins[0];
        Debug.Log("Using label skin based on current device: " + this.LabelSkin);
    }

    private void AutoSetLabelSkinFromProfile()
    {
        var validProfileLabelSkin = LabelSkins.Contains(ProfileData.LastNoteLabels);
        this.LabelSkin = validProfileLabelSkin ? ProfileData.LastNoteLabels : LabelSkins[0];
        Debug.Log("Loaded label skin from profile: " + this.LabelSkin);
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
        this.AllyBoosts = 0;
        this.AllyBoostTicks = 0;
        this.TicksForNextBoost = TICKS_FOR_FIRST_ALLY_BOOST;
        this.AllyBoostsReceived = 0;
        this.AllyBoostsProvided = 0;
        this.Ranking = 1;
    }

    public int GetBaseExpGain()
    {
        float result = PerfPoints;
        return (int)result;
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
        return LocalSlot > 0 ? "P" + Slot : "None";
    }

    public void ApplyExpGain(float modifier)
    {
        this.Exp += (int)(GetBaseExpGain() * modifier);
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
            MaxCombo = this.MaxCombo,
            FullComboType = this.GetFullComboType()
        };
        return result;
    }

    public FullComboType GetFullComboType()
    {
        if (!IsLocalPlayer)
        {
            return NetFullComboType;
        }

        var totalNotes = this.EarlyHits.Sum(e => e.Value)
                         + this.LateHits.Sum(e => e.Value)
                         + this.Mistakes[JudgeResult.Miss];     // Include misses, but not wrongs.

        var perfectHits = this.EarlyHits[JudgeResult.Perfect] + this.LateHits[JudgeResult.Perfect]
                                                              + this.EarlyHits[JudgeResult.Crit] +
                                                              this.LateHits[JudgeResult.Crit];

        if (totalNotes == 0)
        {
            return FullComboType.None;
        }
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

    public void TriggerRumbleForTitleStart()
    {
        _inputManager.TriggerRumble(0.5f, 0.5f);
    }

    public void TriggerRumbleForWrongInput()
    {
        _inputManager.TriggerRumble(0.65f, 0.2f);
    }

}
