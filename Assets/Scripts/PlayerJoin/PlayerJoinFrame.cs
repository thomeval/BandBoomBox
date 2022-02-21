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

        }
    }

    private PlayerManager _playerManager;
    public Color ErrorMessageColor = new Color(1.0f, 0.5f, 0.5f);

    [Header("Sounds")]
    public AudioSource SfxSelectionConfirmed;
    public AudioSource SfxSelectionCancelled;
    public AudioSource SfxMistake;

    public event EventHandler PlayerLeft;

    void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }
    // Start is called before the first frame update
    void Start()
    {

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
    }

    public void HandleInput(InputEvent inputEvent)
    {
        switch (State)
        {
            case PlayerJoinState.Ready:
                if (inputEvent.Action == "B" || inputEvent.Action == "Back")
                {
                    State = PlayerJoinState.Options;
                    SfxSelectionCancelled.PlayUnlessNull();
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
        SfxSelectionCancelled.PlayUnlessNull();
        PlayerLeft?.Invoke(this, null);
    }

    public void AssignPlayer(Player player, bool withSfx)
    {
        this.Player = player;
        this.ProfileSelectFrame.PopulateProfileList();
        this.State = string.IsNullOrEmpty(player.ProfileId) ? PlayerJoinState.ProfileSelect : PlayerJoinState.Options;


        if (withSfx)
        {
            this.SfxSelectionConfirmed.PlayUnlessNull();
        }

        this.Refresh();
    }

    public void TrySetProfileToPlayer(ProfileData profileData)
    {

        if (!_playerManager.ProfileAvailable(profileData.ID, this.Player.Slot))
        {
            SfxMistake.PlayUnlessNull();
            ProfileSelectFrame.Error = "The selected profile is in use by another player.";
            return;
        }

        ProfileSelectFrame.Error = null;
        profileData.ApplyToPlayer(this.Player);
        State = PlayerJoinState.Options;
        SfxSelectionConfirmed.PlayUnlessNull();
        Refresh();
    }
}
