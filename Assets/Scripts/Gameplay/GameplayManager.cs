using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : ScreenManager
{

    public List<NoteManager> NoteManagers;
    public NoteGenerator NoteGenerator;
    public HudManager HudManager;
    public PauseMenu PauseMenu;
    public SongStarScoreValues SongStarScoreValues;

    public float SongPosition
    {
        get { return _songManager.GetSongPosition(); }
    }

    public float SongPositionInBeats
    {
        get { return _songManager.GetSongPositionInBeats(); }
    }

    public long Score;
    public int TeamCombo;
    public int MaxTeamCombo;

    public double Multiplier;
    public double MaxMultiplier;

    [SerializeField]
    private float _energy;

    public float Energy
    {
        get { return _energy; }
        set
        {
            value = Mathf.Clamp(value, 0.0f, MaxEnergy);
            _energy = value;

            if (_energy == 0.0f)
            {
                if (_playerManager.AnyTurboActive())
                {
                    PlaySfx(SoundEvent.Gameplay_TurboOff);
                }
                DisableAllTurbos();
            }

            HudManager.UpdateEnergy(_energy, _playerManager.AnyTurboActive());
        }
    }

    private void DisableAllTurbos()
    {
        _playerManager.DisableAllTurbos();

        foreach (var note in NoteManagers)
        {
            note.TurboActive = false;
        }
        HudManager.UpdateEnergy(_energy, _playerManager.AnyTurboActive());
    }

    public float MaxEnergy
    {
        get { return _playerManager.Players.Count; }
    }
    public float MxGainRate = 1.0f;
    public GameplayState GameplayState = GameplayState.Intro;
    public GameObject NoteHighways;

    public const float MX_COMBO_FOR_GAIN_BONUS = 50;
    public const double MX_MINIMUM = 0.1;

    public const float ENERGY_DRAIN_RATE = 1.0f / 12;
    public const double OUTRO_TIME = 2.0;

    /// <summary>
    /// Scale applied to the note highways when there are four players.
    /// </summary>
    public const float FOUR_PLAYER_HUD_SCALE = 0.8f;

    /// <summary>
    /// Vertical spacing applied to the note highways when there are four players.
    /// </summary>
    public const int FOUR_PLAYER_HUD_SPACING = -10;

    private PlayerManager _playerManager;
    private SongManager _songManager;
    private SongStarValueCalculator _songStarValueCalculator;
    private BackgroundManager _backgroundManager;

    private HitJudge _hitJudge;
    DateTime _lastUpdate = DateTime.Now;
    private DateTime _outroTime;
    private bool _isSongLoading;
    private readonly float[] _turborMxGainRates = { 0.0f, 1.0f, 2.5f, 4.5f, 7.0f, 10.0f, 11.0f, 13.0f, 16f };

    void Awake()
    {
        if (!FindCoreManager())
        {
            return;
        }

        _hitJudge = new HitJudge();
        _playerManager = CoreManager.PlayerManager;
        _songManager = FindObjectOfType<SongManager>();
        _songStarValueCalculator = FindObjectOfType<SongStarValueCalculator>();
        _backgroundManager = FindObjectOfType<BackgroundManager>();
    }

    private void SetupNoteHighways()
    {
        foreach (var noteManager in NoteManagers.Where(e => e.ParentEnabled))
        {
            var player = _playerManager.Players.Single(e => e.Slot == noteManager.Slot);
            NoteGenerator.LoadOrGenerateSongNotes(_songManager.CurrentSong, player.ChartGroup, player.Difficulty, noteManager);
            NoteGenerator.GenerateBeatLines(player.BeatLineType, _songManager.CurrentSong, noteManager);
            noteManager.ApplyNoteSkin(player.NoteSkin, player.LabelSkin);
            noteManager.CalculateAbsoluteTimes(_songManager.CurrentSong.Bpm);
            noteManager.ScrollSpeed = player.ScrollSpeed;
            _playerManager.SetMaxPerfPoints(noteManager.MaxPerfPoints, player.Slot);
            noteManager.ScrollingBackgroundOpacity = 0.0f;
        }
    }

    private void AssignManagers()
    {
        int num = 0;
        bool multiplayer = _playerManager.Players.Count > 1;
        foreach (var noteManager in NoteManagers)
        {
            noteManager.ParentEnabled = false;
        }

        foreach (var player in _playerManager.Players.Where(e => e.Slot == 0))
        {
            player.gameObject.SetActive(false);
        }

        foreach (var player in _playerManager.GetLocalPlayers())
        {
            var pHudManager = HudManager.PlayerHudManagers[num];
            pHudManager.ShowRankings = multiplayer;
            pHudManager.Slot = player.Slot;
            player.HudManager = HudManager.PlayerHudManagers[num];
            NoteManagers[num].Slot = player.Slot;
            NoteManagers[num].ParentEnabled = true;

            num++;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        CoreManager.SongPreviewManager.StopPreviews();

        var fourPlayers = _playerManager.GetLocalPlayers().Count == 4;
        var highwayScale = fourPlayers ? FOUR_PLAYER_HUD_SCALE : 1.0f;
        NoteHighways.transform.localScale = new Vector3(highwayScale, highwayScale, highwayScale);
        NoteHighways.GetComponent<VerticalLayoutGroup>().spacing = fourPlayers ? FOUR_PLAYER_HUD_SPACING : 0;
        AssignManagers();

        var selectedSongData = CoreManager.CurrentSongData;
        ApplySelectedSong(selectedSongData);
        SetupNoteHighways();
        CalculateStarScores();

        // Temporary value. This will get set properly once the song goes past its playable state.
        _outroTime = DateTime.Now.AddDays(1);

        _lastUpdate = DateTime.Now;

        UpdateRankings();

        _playerManager.Reset();
        this.DisableAllTurbos();
        this.Energy = 0.0f;
        HudManager.EnergyMeter.MaxEnergy = this.MaxEnergy;
        GameplayState = GameplayState.Intro;
    }

    private void CalculateStarScores()
    {
        var activeManagers = NoteManagers.Where(e => e.gameObject.activeInHierarchy).ToList();
        SongStarScoreValues = _songStarValueCalculator.CalculateSuggestedScores(activeManagers);
    }

    void FixedUpdate()
    {
        var timeDiff = (DateTime.Now - _lastUpdate).TotalSeconds;
        if (this.Multiplier > 1.0f)
        {
            DecayMultiplier(timeDiff);
        }
        else
        {
            RecoverMultiplier(timeDiff);
        }

        UpdatePlayerEnergy(timeDiff);
        UpdateMxGainRate();
        UpdateGameplayState();
        CheckOutroState();
        UpdateBackground();
        _lastUpdate = DateTime.Now;

    }

    private void UpdateBackground()
    {
        var backgroundSpeed = Math.Min(1.0, this.Multiplier);

        _backgroundManager.SetSpeedMultiplier((float)backgroundSpeed);
        _backgroundManager.SetBeatNumber(_songManager.GetSongPositionInBeats());
    }

    private void UpdatePlayerEnergy(double timeDiff)
    {
        if (this.GameplayState == GameplayState.Paused)
        {
            return;
        }

        if (!_playerManager.AnyTurboActive())
        {
            return;
        }
        var amount = (float)(timeDiff * ENERGY_DRAIN_RATE);
        var playersUsingTurbo = _playerManager.Players.Count(e => e.TurboActive);
        amount *= playersUsingTurbo;

        Energy -= amount;

    }

    private void CheckOutroState()
    {
        if (DateTime.Now > _outroTime)
        {
            SceneTransition(GameScene.Evaluation);
        }
    }

    private void UpdateGameplayState()
    {
        if (GameplayState == GameplayState.Paused || GameplayState == GameplayState.Outro)
        {
            return;
        }

        // Note: Audio clip will rewind automatically once it is done playing.
        if (SongPosition < 0 && _songManager.IsSongPlaying)
        {
            GameplayState = GameplayState.Intro;
        }
        else if (SongPosition > _songManager.GetPlayableLength())
        {
            GameplayState = GameplayState.Outro;
            _outroTime = DateTime.Now.AddSeconds(OUTRO_TIME);
            CoreManager.LastTeamScore = GetTeamScore();
        }
        else
        {
            GameplayState = GameplayState.Playing;
        }

    }

    private void RecoverMultiplier(double timeDiff)
    {
        if (GameplayState == GameplayState.Paused)
        {
            return;
        }

        this.Multiplier = GameplayUtils.RecoverMultiplier(this.Multiplier, timeDiff);
    }

    private void DecayMultiplier(double timeDiff)
    {
        if (GameplayState == GameplayState.Paused)
        {
            return;
        }

        this.Multiplier = GameplayUtils.DecayMultiplier(this.Multiplier, timeDiff);
    }

    private void SongManager_SongLoaded()
    {
        _songManager.StartSong();
        _isSongLoading = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Avoids a race condition where this method could be called before the song has loaded (especially when restarting a song).
        if (_isSongLoading)
        {
            return;
        }

        foreach (var noteManager in NoteManagers)
        {
            noteManager.SongPosition = SongPosition;
            noteManager.SongPositionInBeats = SongPositionInBeats;
            noteManager.UpdateNotes();
        }

    }
    public override void OnPlayerInput(InputEvent inputEvent)
    {
        if (GameplayState == GameplayState.Paused)
        {
            HandlePauseMenuInput(inputEvent);
            return;

        }
        if (inputEvent.IsPressed)
        {
            var player = _playerManager.GetPlayer(inputEvent.Player);
            if (inputEvent.Action == InputAction.Pause || inputEvent.Action == InputAction.Back)
            {
                PauseGame(inputEvent.Player, true);
                return;
            }

            if (inputEvent.Action == InputAction.Turbo)
            {
                ToggleTurbo(player);
                return;
            }

            var noteType = NoteUtils.GetNoteTypeForInput(inputEvent.Action);

            var noteManager = GetNoteManager(inputEvent.Player);


            if (noteType != null && noteManager != null)
            {
                var lane = NoteUtils.GetNoteLane(noteType.Value);
                var playerHudManager = HudManager.GetPlayerHudManager(player.Slot);
                playerHudManager.FlashLane(lane);
                var note = noteManager.FindNextNote(noteType.Value, true);

                if (note != null)
                {
                    var allowCrit = _playerManager.GetPlayer(inputEvent.Player).TurboActive;

                    // Note was hit. Apply a hit result.
                    var deviation = SongPosition - note.AbsoluteTime;
                    var hitResult = _hitJudge.GetHitResult(deviation, inputEvent.Player, player.Difficulty, lane, note.NoteType, note.NoteClass, allowCrit);
                    ApplyHitResult(hitResult);
                    if (note.NoteClass == NoteClass.Hold)
                    {
                        noteManager.OnNoteHeld(note.Lane);
                        playerHudManager.DisplayHeldNote(note.EndNote);
                    }

                    if (hitResult.JudgeResult < JudgeResult.Ok)
                    {
                        playerHudManager.FlashNoteHit(lane);
                    }

                    noteManager.RemoveNote(note);
                }
                else
                {
                    // Check for WRONG judgement
                    note = noteManager.FindNextNote(true);
                    if (note != null)
                    {
                        var hitResult = _hitJudge.GetWrongResult(lane, inputEvent.Player, player.Difficulty);
                        ApplyHitResult(hitResult);
                    }
                }
            }
        }
        else
        {
            HandlePlayerReleaseInput(inputEvent);
        }
    }

    public override void OnDeviceLost(DeviceLostArgs args)
    {
        PauseGame(args.Player, true);
    }

    private const float MIN_ENERGY_FOR_TURBO = 0.15f;
    private void ToggleTurbo(Player player)
    {
        var activating = !player.TurboActive;
        if (Energy < MIN_ENERGY_FOR_TURBO && activating)
        {
            return;
        }

        else if (!activating)
        {
            PlaySfx(SoundEvent.Gameplay_TurboOff);
            Energy -= MIN_ENERGY_FOR_TURBO;
            player.ToggleTurbo();
        }
        else
        {
            PlaySfx(SoundEvent.Gameplay_TurboOn);
            player.ToggleTurbo();
        }

        GetNoteManager(player.Slot).TurboActive = player.TurboActive;
        HudManager.UpdateEnergy(Energy, _playerManager.AnyTurboActive());
    }

    private void HandlePlayerReleaseInput(InputEvent inputEvent)
    {
        var noteType = NoteUtils.GetNoteTypeForInput(inputEvent.Action);

        if (noteType == null)
        {
            return;
        }

        var player = _playerManager.GetPlayer(inputEvent.Player);
        var playerHudManager = HudManager.GetPlayerHudManager(inputEvent.Player);
        var lane = NoteUtils.GetNoteLane(noteType.Value);

        var noteManager = GetNoteManager(inputEvent.Player);

        var releaseNote = noteManager.OnNoteReleased(lane);

        if (releaseNote != null)
        {
            var allowCrit = _playerManager.GetPlayer(inputEvent.Player).TurboActive;
            var deviation = SongPosition - releaseNote.AbsoluteTime;
            var hitResult = _hitJudge.GetHitResult(deviation, inputEvent.Player, player.Difficulty, lane, releaseNote.NoteType, releaseNote.NoteClass, allowCrit);
            ApplyHitResult(hitResult);

            if (hitResult.JudgeResult < JudgeResult.Ok)
            {
                playerHudManager.FlashNoteHit(lane);
            }
        }

        playerHudManager.ReleaseLane(lane);
    }

    private void HandlePauseMenuInput(InputEvent inputEvent)
    {
        PauseMenu.HandleInput(inputEvent);
    }

    private void PauseGame(int player, bool pause)
    {
        if (pause)
        {
            if (GameplayState == GameplayState.Outro)
            {
                return;
            }
            PauseMenu.Show(player);
            GameplayState = GameplayState.Paused;
            _songManager.PauseSong(true);

            foreach (var phudManager in HudManager.PlayerHudManagers)
            {
                phudManager.ReleaseAllLanes();
            }
        }
        else
        {

            if (player != PauseMenu.Player)
            {
                return;
            }
            PauseMenu.Hide();
            GameplayState = GameplayState.Playing;
            _songManager.PauseSong(false);
        }

    }

    void PauseMenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Continue":
                PauseGame(args.Player, false);
                break;
            case "Restart":
                SceneTransition(GameScene.Gameplay);
                break;
            case "Change Difficulty":
                SceneTransition(GameScene.DifficultySelect);
                break;
            case "Exit Song":
                SceneTransition(GameScene.SongSelect);
                break;
        }
    }

    private NoteManager GetNoteManager(int player)
    {
        return NoteManagers.FirstOrDefault(e => e.Slot == player);
    }

    public override void OnPlayerControlsChanged(ControlsChangedArgs args)
    {
        var noteManager = GetNoteManager(args.Player);
        noteManager.ApplyNoteSkin(args.ControllerType);
    }

    private void ApplyHitResult(HitResult hitResult)
    {
        var appliedMxGainRate = hitResult.DeviationResult == DeviationResult.NotHit ? 1.0f : MxGainRate;

        hitResult.ScorePoints = (int)(this.Multiplier * hitResult.ScorePoints);
        this.Score += hitResult.ScorePoints;
        this.Multiplier += hitResult.MxPoints * appliedMxGainRate;
        this.Multiplier = Math.Max(MX_MINIMUM, this.Multiplier);
        this.MaxMultiplier = Math.Max(this.Multiplier, this.MaxMultiplier);

        const float ENERGY_GAIN_RATE = 0.01f;
        if (hitResult.JudgeResult <= JudgeResult.Perfect)
        {
            this.Energy += ENERGY_GAIN_RATE;
        }
        _playerManager.ApplyHitResult(hitResult, hitResult.Player);
        UpdateTeamCombo(hitResult.JudgeResult);
        UpdateRankings();
    }

    private void UpdateTeamCombo(JudgeResult result)
    {
        var comboBreak = HitJudge.IsComboBreak(result);
        if (!comboBreak.HasValue)
        {
            return;
        }

        if (comboBreak.Value)
        {
            this.TeamCombo = 0;
        }
        else
        {
            this.TeamCombo++;
            this.MaxTeamCombo = Math.Max(this.MaxTeamCombo, this.TeamCombo);
        }

        UpdateMxGainRate();
    }

    private void UpdateMxGainRate()
    {
        var newGainRate = 1.0f;

        var comboGainBonus = ((int)(TeamCombo / MX_COMBO_FOR_GAIN_BONUS)) * 0.05f;
        comboGainBonus = Math.Min(comboGainBonus, 1.0f);
        newGainRate += comboGainBonus;

        var playersInTurbo = _playerManager.Players.Count(e => e.TurboActive);
        var turboBonus = _turborMxGainRates[playersInTurbo];
        newGainRate += turboBonus;
        MxGainRate = newGainRate;
    }


    private void UpdateRankings()
    {
        var orderedPlayers = _playerManager.Players.OrderByDescending(e => e.PerfPercent).ToList();
        int count = 0;
        float last = 1000.0f;
        for (int x = 0; x < orderedPlayers.Count; x++)
        {
            var perc = orderedPlayers[x].PerfPercent;
            if (perc < last)
            {
                last = perc;
                count++;
            }
            orderedPlayers[x].Ranking = count;
        }

    }

    private void ApplySelectedSong(SongData selectedSongData)
    {
        _isSongLoading = true;
        _songManager.LoadSong(selectedSongData, SongManager_SongLoaded);
        HudManager.SongTitleText = selectedSongData.Title + " " + selectedSongData.Subtitle;
    }

    public void OnNoteMissed(Note note, int player)
    {
        var diff = _playerManager.GetPlayer(player).Difficulty;
        var result = _hitJudge.GetMissResult(note.Lane, player, diff);
        ApplyHitResult(result);
    }

    private TeamScore GetTeamScore()
    {
        return new TeamScore
        {
            SongId = _songManager.CurrentSong.ID,
            SongTitle = string.Format("{0} {1}", _songManager.CurrentSong.Title, _songManager.CurrentSong.Subtitle),
            SongArtist = _songManager.CurrentSong.Artist,
            SongVersion = _songManager.CurrentSong.Version,
            DateTime = DateTime.Now,
            MaxMultiplier = this.MaxMultiplier,
            MaxTeamCombo = this.MaxTeamCombo,
            NumPlayers = _playerManager.Players.Count,
            Category = HighScoreManager.GetCategory(_playerManager.Players.Count),
            Score = this.Score,
            Stars = GetStarFraction(this.Score)
        };
    }

    public double GetStarFraction(long score)
    {
        if (SongStarScoreValues == null)
        {
            return 0;
        }
        return SongStarScoreValues.GetStarFraction(score);
    }
}
