using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinOptionsFrame : MonoBehaviour
{
    public PlayerJoinFrame Parent;
    public Menu Menu;

    public Text TxtScrollSpeed;
    public Text TxtTimingDisplay;
    public Text TxtLabelSkin;
    public Text TxtGoal;
    public Text TxtAllyBoostsEnabled;
    public Text TxtMistakeSfxEnabled;
    public Text TxtControllerRumbleEnabled;
    public Text TxtSectionDifficulty;
    public Text TxtMomentum;
    public Text TxtLaneOrderType;

    public GameObject MomentumMenuItem;
    public GameObject AllyBoostMenuItem;
    public GameObject SectionDifficultyMenuItem;
    public GameObject LaneOrderTypeMenuItem;

    public Grade?[] Goals = { null, Grade.D, Grade.DPlus, Grade.C, Grade.CPlus, Grade.B, Grade.BPlus, Grade.A, Grade.APlus, Grade.S, Grade.SPlus };
    public int[] MomentumAmounts = { 0, 10, 25, 50, 100};
    NoteType[] _noteTypesInPreview = { NoteType.A, NoteType.B, NoteType.X, NoteType.Y, NoteType.Down, NoteType.Right, NoteType.Left, NoteType.Up };
    public List<Note> NotePreviews;

    void OnEnable()
    {
        UpdateNotePreviews();
        UpdateMenu();
        Menu.FullRefresh();
        Menu.RefreshHighlight();
        Menu.UpdateExplanationText();
    }

    public void HandleInput(InputEvent inputEvent)
    {
        Menu.HandleInput(inputEvent);
        UpdateMenu();

    }

    void MenuItemShifted(MenuEventArgs args)
    {
        var amount = args.ShiftAmount;
        switch (args.SelectedItem)
        {
            case "Scroll Speed":
                Parent.Player.ChangeScrollSpeed(amount);
                break;
            case "Timing Display":
                Parent.Player.ChangeTimingDisplayType(amount);
                break;
            case "Note Labels":
                Parent.Player.ChangeLabelSkin(amount);
                UpdateNotePreviews();
                break;
            case "Goal":
                var newGrade = Helpers.GetNextValue(Goals, Parent.Player.GetGoalGrade(), amount, false);
                Parent.Player.Goal = Helpers.GradeToPercent(newGrade);
                break;
            case "Mistake Sfx":
                Parent.Player.MistakeSfxEnabled = !Parent.Player.MistakeSfxEnabled;
                break;
            case "Controller Rumble":
                Parent.Player.RumbleEnabled = !Parent.Player.RumbleEnabled;
                break;
            case "Momentum Speed":
                var newMomentum = Helpers.GetNextValue(MomentumAmounts, Parent.Player.Momentum, amount, false);
                Parent.Player.Momentum = newMomentum;
                break;
                case "Ally Boosts":
                Parent.Player.ProfileData.AllyBoostMode = Helpers.GetNextEnumValue(Parent.Player.ProfileData.AllyBoostMode, amount, false);
                break;
            case "Section Difficulty":
                Parent.Player.ProfileData.SectionDifficulty = Helpers.GetNextEnumValue(Parent.Player.ProfileData.SectionDifficulty, amount, false);
                break;
                case "Lane Order":
                Parent.Player.LaneOrderType = Helpers.GetNextEnumValue(Parent.Player.LaneOrderType, amount, false);
                break;
        }
    }

    void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Ready":
                Parent.State = PlayerState.PlayerJoin_Ready;
                break;
            case "Select Profile":
                Parent.ProfileSelectFrame.PopulateProfileList();
                Parent.ProfileSelectFrame.Refresh();
                Parent.State = PlayerState.PlayerJoin_SelectProfile;
                break;
            case "Leave":
                Parent.State = PlayerState.NotPlaying;
                Parent.RemovePlayer();
                break;
        }
    }

    public void UpdateMenu()
    {

        var player = Parent.Player;

        if (player == null)
        {
            return;
        }

        TxtScrollSpeed.text = "" + player.ScrollSpeed;
        TxtLabelSkin.text = player.LabelSkin;
        UpdateNotePreviews();
        TxtTimingDisplay.text = player.TimingDisplayType.ToString();

        UpdateGoalText(player);
        TxtMistakeSfxEnabled.text = BoolToOnOff(player.MistakeSfxEnabled);
        TxtControllerRumbleEnabled.text = BoolToOnOff(player.RumbleEnabled);
        TxtMomentum.text = "" + player.Momentum;
        TxtAllyBoostsEnabled.text = player.ProfileData.AllyBoostMode.ToString();
        TxtSectionDifficulty.text = player.ProfileData.SectionDifficulty.ToString();
        TxtLaneOrderType.text = player.LaneOrderType.ToString();
    }

    private string BoolToOnOff(bool value)
    {
        return value ? "On" : "Off";
    }

    private void UpdateGoalText(Player player)
    {
        var goalGrade = player.GetGoalGrade();
        string goalText;
        if (goalGrade == null)
        {
            goalText = "No Goal";
        }
        else
        {
            goalText = goalGrade.ToString().Replace("Plus", "+");
            goalText += $" ({Helpers.GradeToPercent(goalGrade):P0})";
        }

        TxtGoal.text = goalText;
    }

    private void UpdateNotePreviews()
    {
        if (Parent.Player == null)
        {
            return;
        }

        var x = 0;
        foreach (var note in NotePreviews)
        {
            note.NoteBase.NoteType = _noteTypesInPreview[x];
            note.SetSpriteCategories(Parent.Player.NoteSkin, Parent.Player.LabelSkin);
            x++;
        }
    }

    private void UpdateLanePreview()
    {
        if (Parent.Player == null)
        {
            return;
        }
    }

    public void ShowMomentumOption()
    {
        MomentumMenuItem.SetActive(true);
    }

    public void ToggleMenuOptions(bool showMomentum, bool showAllyBoost, bool showSectionDifficulty, bool showLaneOrder)
    {
        MomentumMenuItem.SetActive(showMomentum);
        AllyBoostMenuItem.SetActive(showAllyBoost);
        SectionDifficultyMenuItem.SetActive(showSectionDifficulty);
        LaneOrderTypeMenuItem.SetActive(showLaneOrder);
    }
}
