using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : ScreenManager
{

    public List<NoteManager> NoteManagers;
    public NoteGenerator NoteGenerator;
    public HudManager HudManager;
    public PauseMenu PauseMenu;
    public SongStarScoreValues SongStarScoreValues = new();
    public OnlinePlayerList OnlinePlayerList;
    public GameplayStateHelper StateHelper;
    public GameplayStateValues StateValues;

    public float SongPosition
    {
        get { return _songManager.GetSongPosition(); }
    }

    public float SongPositionInBeats
    {
        get { return _songManager.GetSongPositionInBeats(); }
    }

    public void DisableAllTurbos()
    {
        if (_playerManager.AnyTurboActive())
        {
            PlaySfx(SoundEvent.Gameplay_TurboOff);
        }
        _playerManager.DisableAllTurbos();

        foreach (var note in NoteManagers)
        {
            note.TurboActive = false;
        }

        HudManager.UpdateEnergyMeter(_playerManager.AnyTurboActive());
    }

    public GameplayScreenState GameplayState = GameplayScreenState.Intro;
    public GameObject NoteHighways;

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
    private bool _startSignalReceived = false;

    private DateTime _songLoadStart;

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
            var player = _playerManager.GetLocalPlayers().Single(e => e.Slot == noteManager.Slot);
            NoteGenerator.LoadOrGenerateSongNotes(_songManager.CurrentSong, player.ChartGroup, player.Difficulty, noteManager);
            NoteGenerator.GenerateBeatLines(_songManager.CurrentSong, noteManager);
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

        SetNoteHighwayScale();

        OnlinePlayerList.gameObject.SetActive(CoreManager.IsNetGame);
        OnlinePlayerList.RefreshAll();
        HudManager.UpdateStarsWithScore = CoreManager.IsHost;

        AssignManagers();

        _playerManager.Reset();

        // Set the team score category now based on the number of players present when the song starts. Should any players join or leave mid-game, this category should not change.
        StateValues.TeamScoreCategory = _playerManager.GetScoreCategory();


        InitTurbo();

        if (!CoreManager.IsNetGame)
        {
            _startSignalReceived = true;
        }

        var selectedSongData = CoreManager.CurrentSongData;
        StartLoadingSong(selectedSongData);
        SetupNoteHighways();

        // Temporary value. This will get set properly once the song goes past its playable state.
        _outroTime = DateTime.Now.AddDays(1);
        _lastUpdate = DateTime.Now;

        GameplayState = GameplayScreenState.Intro;

    }

    private void SetNoteHighwayScale()
    {
        var fourPlayers = _playerManager.GetLocalPlayers().Count == 4;
        var highwayScale = fourPlayers ? FOUR_PLAYER_HUD_SCALE : 1.0f;
        NoteHighways.transform.localScale = new Vector3(highwayScale, highwayScale, highwayScale);
        NoteHighways.GetComponent<VerticalLayoutGroup>().spacing = fourPlayers ? FOUR_PLAYER_HUD_SPACING : 0;
    }

    private void InitTurbo()
    {
        this.DisableAllTurbos();

        StateValues.Energy = 0.0f;
        StateValues.MaxEnergy = _playerManager.Players.Count(e => e.IsParticipating);
        HudManager.EnergyMeter.MaxEnergy = StateValues.MaxEnergy;
    }

    private void CalculateStarScoresFromSongData()
    {
        SongStarScoreValues = _songStarValueCalculator.CalculateSuggestedScores(CoreManager.CurrentSongData);
        HudManager.SongStarScoreValues = SongStarScoreValues;
    }

    void FixedUpdate()
    {
        var timeDiff = (DateTime.Now - _lastUpdate).TotalSeconds;

        StateHelper.UpdateGameplayState(timeDiff);
        UpdateGameplayScreenState();
        CheckOutroState();
        UpdateBackground();
        UpdateScrollSpeeds();
        _lastUpdate = DateTime.Now;
    }

    private void UpdateScrollSpeeds()
    {
        foreach (var manager in NoteManagers)
        {
            var player = _playerManager.GetLocalPlayers().SingleOrDefault(e => e.Slot == manager.Slot);
            if (player == null)
            {
                continue;
            }

            manager.ScrollSpeed = player.GetCurrentScrollSpeed(StateValues.Multiplier);
        }
    }

    private void UpdateBackground()
    {
        var backgroundSpeed = Math.Min(1.0, StateValues.Multiplier);

        _backgroundManager.SetSpeedMultiplier((float)backgroundSpeed);
        _backgroundManager.SetBeatNumber(_songManager.GetSongPositionInBeats());
    }

    private void CheckOutroState()
    {
        if (DateTime.Now > _outroTime)
        {
            SceneTransition(GameScene.Evaluation);
        }
    }

    private void UpdateGameplayScreenState()
    {
        if (GameplayState == GameplayScreenState.Paused || GameplayState == GameplayScreenState.Outro)
        {
            return;
        }

        // Note: Audio clip will rewind automatically once it is done playing.
        if (SongPosition < 0 && _songManager.IsSongPlaying)
        {
            GameplayState = GameplayScreenState.Intro;
        }
        else if (SongPosition > _songManager.GetPlayableLength())
        {
            GameplayState = GameplayScreenState.Outro;
            _outroTime = DateTime.Now.AddSeconds(OUTRO_TIME);
            CoreManager.LastTeamScore = GetTeamScore();
        }
        else
        {
            GameplayState = GameplayScreenState.Playing;
        }

    }

    private void SongManager_SongLoaded()
    {
        _isSongLoading = false;
        UpdatePlayersState(PlayerState.Gameplay_ReadyToStart);
        var songLoadTime = DateTime.Now - _songLoadStart;
        Debug.Log(string.Format("Song loaded in {0:F0}ms", songLoadTime.TotalMilliseconds));
        CalculateStarScoresFromSongData();

        // Send the max possible base score (calculated above) to each client so that they can calculate stars earned correctly.
        SendNetGameplayStateValuesUpdate(StateValues.AsDto());
        TryToStartSong();
    }

    private void TryToStartSong()
    {
        if (_isSongLoading || !_startSignalReceived)
        {
            return;
        }

        _songManager.StartSong();
        UpdatePlayersState(PlayerState.Gameplay_Playing);
    }

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
        if (GameplayState == GameplayScreenState.Paused)
        {
            HandlePauseMenuInput(inputEvent);
            return;

        }
        if (inputEvent.IsPressed)
        {
            var player = _playerManager.GetLocalPlayer(inputEvent.Player);
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

            HandleNoteInput(inputEvent, player);
        }
        else
        {
            HandlePlayerReleaseInput(inputEvent);
        }
    }

    private void HandleNoteInput(InputEvent inputEvent, Player player)
    {
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
                var allowCrit = _playerManager.GetLocalPlayer(inputEvent.Player).TurboActive;

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

    public override void OnDeviceLost(DeviceLostArgs args)
    {
        PauseGame(args.Player, true);
    }

    private void ToggleTurbo(Player player)
    {
        var activating = !player.TurboActive;
        if (StateValues.Energy < StateHelper.MIN_ENERGY_FOR_TURBO && activating)
        {
            return;
        }

        else if (!activating)
        {
            PlaySfx(SoundEvent.Gameplay_TurboOff);
            StateValues.Energy -= StateHelper.MIN_ENERGY_FOR_TURBO;
            player.ToggleTurbo();
        }
        else
        {
            PlaySfx(SoundEvent.Gameplay_TurboOn);
            player.ToggleTurbo();
        }

        GetNoteManager(player.Slot).TurboActive = player.TurboActive;
        HudManager.UpdateEnergyMeter(_playerManager.AnyTurboActive());
    }

    private void HandlePlayerReleaseInput(InputEvent inputEvent)
    {
        var noteType = NoteUtils.GetNoteTypeForInput(inputEvent.Action);

        if (noteType == null)
        {
            return;
        }

        var player = _playerManager.GetLocalPlayer(inputEvent.Player);
        var playerHudManager = HudManager.GetPlayerHudManager(inputEvent.Player);
        var lane = NoteUtils.GetNoteLane(noteType.Value);

        var noteManager = GetNoteManager(inputEvent.Player);

        var releaseNote = noteManager.OnNoteReleased(lane);

        if (releaseNote != null)
        {
            var allowCrit = _playerManager.GetLocalPlayer(inputEvent.Player).TurboActive;
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
        // Don't allow pausing in net games
        if (CoreManager.IsNetGame)
        {
            return;
        }

        if (pause)
        {
            if (GameplayState == GameplayScreenState.Outro)
            {
                return;
            }
            PauseMenu.Show(player);
            GameplayState = GameplayScreenState.Paused;
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
            GameplayState = GameplayScreenState.Playing;
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
        var player = _playerManager.GetLocalPlayer(hitResult.PlayerSlot);
        _playerManager.ApplyHitResult(hitResult, hitResult.PlayerSlot);
        _playerManager.UpdateRankings();
        SendNetPlayerScoreUpdate(player);
        CoreManager.ServerNetApi.ApplyHitResultServerRpc(hitResult);

        if (!CoreManager.IsNetGame)
        {
            ApplyHitResultToTeam(hitResult);
        }
    }

    private void ApplyHitResultToTeam(HitResult hitResult)
    {
        if (!CoreManager.IsHost)
        {
            return;
        }

        StateHelper.ApplyHitResult(hitResult);
        StateHelper.UpdateTeamCombo(hitResult.JudgeResult);
        SendNetGameplayStateValuesUpdate(StateValues.AsDto());
    }

    private void StartLoadingSong(SongData selectedSongData)
    {
        _songLoadStart = DateTime.Now;
        _isSongLoading = true;
        _songManager.LoadSong(selectedSongData, SongManager_SongLoaded);
        HudManager.SongTitleText = selectedSongData.Title + " " + selectedSongData.Subtitle;
    }

    public void OnNoteMissed(Note note, int player)
    {
        var diff = _playerManager.GetLocalPlayer(player).Difficulty;
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
            MaxMultiplier = StateValues.MaxMultiplier,
            MaxTeamCombo = StateValues.MaxTeamCombo,
            NumPlayers = _playerManager.Players.Count(e => e.IsParticipating),
            Category = StateValues.TeamScoreCategory,
            Score = StateValues.Score,
            Stars = StateValues.Stars
        };
    }

    public override void OnNetPlayerListUpdated(bool playerJoined, bool playerLeft)
    {
        base.OnNetPlayerListUpdated(playerJoined, playerLeft);
        OnlinePlayerList.RefreshAll();
    }

    public override void OnNetPlayerUpdated(Player player)
    {
        base.OnNetPlayerUpdated(player);
        OnlinePlayerList.Refresh(player);
    }

    public override void OnNetPlayerScoreUpdated(Player player)
    {
        base.OnNetPlayerScoreUpdated(player);
        OnlinePlayerList.Refresh(player);
    }

    public override void OnNetStartSongSignal()
    {
        base.OnNetStartSongSignal();
        _startSignalReceived = true;
        TryToStartSong();
    }

    public override void OnNetGameplayStateValuesUpdated(GameplayStateValuesDto dto)
    {
        base.OnNetGameplayStateValuesUpdated(dto);
        StateValues.CopyValues(dto);
        HudManager.UpdateEnergyMeter(_playerManager.AnyTurboActive());

        if (CoreManager.IsHost)
        {
            return;
        }
        HudManager.StarMeter.Value = StateValues.Stars;
    }

    public override void OnNetHitResult(HitResult hitResult)
    {
        base.OnNetHitResult(hitResult);
        ApplyHitResultToTeam(hitResult);
    }

    public void SendNetHitResult(HitResult hitResult)
    {
        if (!CoreManager.IsNetGame)
        {
            return;
        }

        CoreManager.ServerNetApi.ApplyHitResultServerRpc(hitResult);
    }

    public void SendNetGameplayStateValuesUpdate(GameplayStateValuesDto dto)
    {
        if (!CoreManager.IsNetGame || !CoreManager.IsHost)
        {
            return;
        }

        CoreManager.ClientNetApi.ReceiveNetGameplayStateValuesClientRpc(dto);
    }

    public override void OnNetShutdown()
    {
        _songManager.PauseSong(true);
        base.OnNetShutdown();
    }
}
