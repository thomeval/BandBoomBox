using System;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinFrame : MonoBehaviour
{

    public PlayerJoinOptionsFrame OptionsFrame;
    public PlayerJoinProfileSelectFrame ProfileSelectFrame;
    public PlayerJoinProfileCreateFrame ProfileCreateFrame;

    public Text TxtPlayerName;

    public GameObject[] Pages;

    public Player Player;
    public ExpMeter ExpMeter;

    [SerializeField]
    private PlayerJoinState _state;

    public PlayerJoinState State
    {
        get { return _state; }
        set
        {
            _state = value;

            foreach (var page in Pages)
            {
                page.SetActive(false);
            }

            Pages[(int)value].SetActive(true);
            _playerJoinManager.RefreshPlayerList();
        }
    }

    public void ShowMomentumOption()
    {
        OptionsFrame.ShowMomentumOption();
    }
    private PlayerManager _playerManager;
    private PlayerJoinManager _playerJoinManager;
    public Color ErrorMessageColor = new Color(1.0f, 0.5f, 0.5f);

    [Header("Sounds")]
    public SoundEventHandler SoundEventHandler;

    public event EventHandler PlayerLeft;

    void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerJoinManager = FindObjectOfType<PlayerJoinManager>();
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

        if (State == PlayerJoinState.Options || State == PlayerJoinState.Ready)
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

    public void HandleInput(InputEvent inputEvent)
    {
        switch (State)
        {
            case PlayerJoinState.Ready:
                if (inputEvent.Action == InputAction.B || inputEvent.Action == InputAction.Back)
                {
                    Player.PlayerState = PlayerState.PlayerJoin_Options;
                    State = PlayerJoinState.Options;
                    SoundEventHandler.PlaySfx(SoundEvent.SelectionCancelled);
                }
                break;
            case PlayerJoinState.Options:
                HandleInputOptionsPage(inputEvent);
                break;
            case PlayerJoinState.ProfileSelect:
                HandleInputProfileSelectPage(inputEvent);
                break;
            case PlayerJoinState.ProfileCreate:
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
        this.Player.PlayerState = string.IsNullOrEmpty(player.ProfileId) ? PlayerState.PlayerJoin_SelectProfile : PlayerState.PlayerJoin_Options;
        this.State = string.IsNullOrEmpty(player.ProfileId) ? PlayerJoinState.ProfileSelect : PlayerJoinState.Options;

        if (withSfx)
        {
            SoundEventHandler.PlaySfx(SoundEvent.SelectionConfirmed);
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
        this.Player.PlayerState = PlayerState.PlayerJoin_Options;
        State = PlayerJoinState.Options;
        SoundEventHandler.PlaySfx(SoundEvent.SelectionConfirmed);
        Refresh();
    }

    public void PlaySfx(SoundEvent soundEvent)
    {
        SoundEventHandler.PlaySfx(soundEvent);
    }
}
