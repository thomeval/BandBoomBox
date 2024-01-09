using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class OnlinePlayerListItem : MonoBehaviour
{
    [Header("Common")]
    public Text TxtPlayerName;
    public Text TxtPlayerStatus;
    public SpriteResolver PlayerIdentifier;
    public Text TxtPlayerLevel;
    public Text TxtNetId;

    public Player Player;

    public virtual void Refresh()
    {
        if (Player == null)
        {
            return;
        }

        SetTextSafe(TxtPlayerName, Player.Name);
        SetTextSafe(TxtPlayerStatus, GetStatusText(Player.PlayerState));

        if (PlayerIdentifier != null)
        {
            PlayerIdentifier.SetCategoryAndLabel("PlayerIdentifiers", Player.GetPlayerIdSprite());
        }

        SetTextSafe(TxtPlayerLevel, $"{ExpLevelUtils.GetLevel(Player.Exp)}");
        SetTextSafe(TxtNetId, $"({Player.DisplayNetId})");

    }

    protected void SetTextSafe(Text textBox, string value)
    {
        if (textBox == null)
        {
            return;
        }

        textBox.text = value;
    }

    protected virtual string GetStatusText(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Unknown:
            case PlayerState.NotPlaying:
                return "";
            case PlayerState.SelectSong:
                return "Selecting a song";
            case PlayerState.PlayerJoin_SelectProfile:
                return "Selecting Profile";
            case PlayerState.PlayerJoin_CreateProfile:
                return "Creating Profile";
            case PlayerState.PlayerJoin_Options:
                return "Setting Player Options";
            case PlayerState.PlayerJoin_Ready:
                return "Waiting For Partner";
            case PlayerState.DifficultySelect_Selecting:
                return "Selecting Difficulty";
            case PlayerState.DifficultySelect_Ready:
                return "Ready";
            case PlayerState.Gameplay_Playing:
                return "Playing";
            case PlayerState.Gameplay_Loading:
                return "Loading";
            case PlayerState.Gameplay_ReadyToStart:
                return "Waiting for other players";
            case PlayerState.Evaluation_NotReady:
                return "Checking Results";
            case PlayerState.Evaluation_Ready:
                return "Ready";
            default:
                throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null);
        }
    }
}
