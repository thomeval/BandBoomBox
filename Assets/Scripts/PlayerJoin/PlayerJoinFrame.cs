using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinFrame : MonoBehaviour
{

    public PlayerJoinOptionsFrame OptionsFrame;
    public PlayerJoinProfileSelectFrame ProfileSelectFrame;
    public PlayerJoinProfileCreateFrame ProfileCreateFrame;

    public Text TxtPlayerName;

    public PlayerState[] PageStates = { PlayerState.NotPlaying, PlayerState.PlayerJoin_SelectProfile, PlayerState.PlayerJoin_CreateProfile, PlayerState.PlayerJoin_Options, PlayerState.PlayerJoin_Ready };
    public GameObject[] Pages;

    public Player Player;
    public ExpMeter ExpMeter;

    public PlayerState State
    {
        get
        {
            return Player?.PlayerState ?? PlayerState.NotPlaying;
        }
        set
        {
            if (Player == null)
            {
                return;
            }

            var valueChanged = Player.PlayerState != value;

            Player.PlayerState = value;
            _playerJoinManager.RefreshPlayerList();

            DisplayCurrentPage(value);

            if (valueChanged)
            {
                SendNetUpdate();
            }
        }
    }

    private PlayerManager _playerManager;
    private PlayerJoinManager _playerJoinManager;
    public Color ErrorMessageColor = new Color(1.0f, 0.5f, 0.5f);

    [Header("Sounds")]
    public MenuSoundEventHandler SoundEventHandler;

    public event EventHandler PlayerLeft;

    void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerJoinManager = FindObjectOfType<PlayerJoinManager>();
        DisplayCurrentPage(PlayerState.NotPlaying);
    }

    public void Refresh()
    {
        if (Player == null)
        {
            TxtPlayerName.text = "";
            ExpMeter.Hide();
            return;
        }
        TxtPlayerName.text = Player.Name;

        if (State == PlayerState.PlayerJoin_Options || State == PlayerState.PlayerJoin_Ready)
        {
            TxtPlayerName.Show();
            ExpMeter.Show();
        }
        else
        {
            TxtPlayerName.Hide();
            ExpMeter.Hide();
        }
        ExpMeter.Exp = Player.Exp;
        OptionsFrame.UpdateMenu();
        ProfileCreateFrame.Reset();
        SendNetUpdate();

    }

    public void SendNetUpdate()
    {
        _playerJoinManager.SendNetPlayerUpdate(this.Player);
    }

    private void DisplayCurrentPage(PlayerState state)
    {
        foreach (var page in Pages)
        {
            page.SetActive(false);
        }

        var idx = Array.IndexOf(PageStates, state);

        if (idx == -1)
        {
            return;
        }
        Pages[idx].SetActive(true);
    }


    public void HandleInput(InputEvent inputEvent)
    {
        switch (State)
        {
            case PlayerState.PlayerJoin_Ready:
                if (inputEvent.Action == InputAction.B || inputEvent.Action == InputAction.Back)
                {
                    Player.PlayerState = PlayerState.PlayerJoin_Options;
                    State = PlayerState.PlayerJoin_Options;
                    SoundEventHandler.PlaySfx(SoundEvent.SelectionCancelled);
                }
                break;
            case PlayerState.PlayerJoin_Options:
                HandleInputOptionsPage(inputEvent);
                break;
            case PlayerState.PlayerJoin_SelectProfile:
                HandleInputProfileSelectPage(inputEvent);
                break;
            case PlayerState.PlayerJoin_CreateProfile:
                ProfileCreateFrame.HandleInput(inputEvent);
                break;
        }
    }

    private void HandleInputProfileSelectPage(InputEvent inputEvent)
    {
        ProfileSelectFrame.HandleInput(inputEvent);
    }

    private void HandleInputOptionsPage(InputEvent inputEvent)
    {
        OptionsFrame.HandleInput(inputEvent);
    }

    public void RemovePlayer()
    {
        SoundEventHandler.PlaySfx(SoundEvent.SelectionCancelled);
        PlayerLeft?.Invoke(this, null);
    }

    public void AssignPlayer(Player player, bool withSfx)
    {
        this.Player = player;
        this.ProfileSelectFrame.PopulateProfileList();
        this.State = string.IsNullOrEmpty(player.ProfileId) ? PlayerState.PlayerJoin_SelectProfile : PlayerState.PlayerJoin_Options;

        if (withSfx)
        {
            PlayConfirmedSfx();
        }

        this.Refresh();
    }

    public void TrySetProfileToPlayer(ProfileData profileData)
    {

        if (!_playerManager.ProfileAvailable(profileData.ID, this.Player.Slot))
        {
            SoundEventHandler.PlaySfx(SoundEvent.Mistake);
            ProfileSelectFrame.Error = "The selected profile is in use by another player.";
            return;
        }

        ProfileSelectFrame.Error = null;
        this.Player.ProfileData = profileData;

        var useControllerNoteLabels = _playerJoinManager.CoreManager.Settings.AutoSetNoteLabelsFromController;
        this.Player.AutoSetLabelSkin(useControllerNoteLabels);
        State = PlayerState.PlayerJoin_Options;
        PlayConfirmedSfx();
        Refresh();
    }

    public void PlaySfx(SoundEvent soundEvent)
    {
        SoundEventHandler.PlaySfx(soundEvent);
    }

    public void PlayConfirmedSfx()
    {
        SoundEventHandler.PlaySfx(SoundEvent.SelectionConfirmed);
    }

    public void ToggleMenuOptions(bool showMomentum, bool showAllyBoost)
    {
        OptionsFrame.ToggleMenuOptions(showMomentum, showAllyBoost);
    }
}
