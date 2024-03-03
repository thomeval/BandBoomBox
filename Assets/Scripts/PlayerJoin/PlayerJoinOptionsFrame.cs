﻿using System;
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
    public Text TxtMistakeSfxEnabled;
    public Text TxtControllerRumbleEnabled;
    public Text TxtMomentum;

    public GameObject MomentumMenuItem;

    public Grade?[] Goals = { null, Grade.D, Grade.DPlus, Grade.C, Grade.CPlus, Grade.B, Grade.BPlus, Grade.A, Grade.APlus, Grade.S, Grade.SPlus };
    public int[] MomentumAmounts = { 0, 25, 50, 100, 150, 200 };
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
        }
    }

    void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Ready":
                Parent.Player.PlayerState = PlayerState.PlayerJoin_Ready;
                Parent.State = PlayerJoinState.Ready;
                Parent.SendNetUpdate();
                break;
            case "Select Profile":
                Parent.ProfileSelectFrame.PopulateProfileList();
                Parent.ProfileSelectFrame.Refresh();
                Parent.Player.PlayerState = PlayerState.PlayerJoin_SelectProfile;
                Parent.State = PlayerJoinState.ProfileSelect;
                Parent.SendNetUpdate();
                break;
            case "Leave":
                Parent.State = PlayerJoinState.NotJoined;
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

        int x = 0;
        foreach (var note in NotePreviews)
        {
            note.NoteBase.NoteType = _noteTypesInPreview[x];
            note.SetSpriteCategories(Parent.Player.NoteSkin, Parent.Player.LabelSkin);
            x++;
        }
    }

    public void ShowMomentumOption()
    {
        MomentumMenuItem.SetActive(true);
    }
}
