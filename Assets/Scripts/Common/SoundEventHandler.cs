﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SoundEventHandler : MonoBehaviour
{
    [Header("Sounds")]

    [Header("Gameplay")]
    public AudioSource SfxGameplayTurboOff;
    public AudioSource SfxGameplayTurboOn;
    public AudioSource SfxMistake;
    public AudioSource[] SfxGameplayStars = new AudioSource[5];

    [Header("Menus")]
    public AudioSource SfxSelectionChanged;
    public AudioSource SfxSelectionConfirmed;
    public AudioSource SfxSelectionCancelled;
    public AudioSource SfxSelectionShifted;

    [Header("Editor")]
    public AudioSource SfxEditorSaveComplete;
    public AudioSource SfxEditorNotePlaced;
    public AudioSource SfxEditorNoteRemoved;
    public AudioSource SfxEditorCut;
    public AudioSource SfxEditorCopy;
    public AudioSource SfxEditorPaste;
    public AudioSource SfxEditorSelectRegionStart;
    public AudioSource SfxEditorSelectRegionEnd;

    [Header("Misc")]
    public AudioSource[] SfxEvaluationGrades = new AudioSource[3];
    public AudioSource SfxTitleScreenStartPressed;
    public AudioSource SfxSecretUnlocked;

    private Dictionary<SoundEvent, AudioSource> _sfxEntries;

    void Awake()
    {
        _sfxEntries = new Dictionary<SoundEvent, AudioSource>()
        {
            { SoundEvent.Mistake, SfxMistake},
            { SoundEvent.SelectionChanged, SfxSelectionChanged},
            { SoundEvent.SelectionConfirmed, SfxSelectionConfirmed},
            { SoundEvent.SelectionCancelled, SfxSelectionCancelled},
            { SoundEvent.SelectionShifted, SfxSelectionShifted},
            { SoundEvent.Editor_SaveComplete, SfxEditorSaveComplete},
            { SoundEvent.Editor_NotePlaced, SfxEditorNotePlaced},
            { SoundEvent.Editor_NoteRemoved, SfxEditorNoteRemoved},

            { SoundEvent.Editor_Cut, SfxEditorCut},
            { SoundEvent.Editor_Copy, SfxEditorCopy},
            { SoundEvent.Editor_Paste, SfxEditorPaste},
            { SoundEvent.Editor_SelectRegionStart, SfxEditorSelectRegionStart},
            { SoundEvent.Editor_SelectRegionEnd, SfxEditorSelectRegionEnd},

            { SoundEvent.Gameplay_TurboOff, SfxGameplayTurboOff},
            { SoundEvent.Gameplay_TurboOn, SfxGameplayTurboOn},

            { SoundEvent.TitleScreen_StartPressed, SfxTitleScreenStartPressed},
            { SoundEvent.SecretUnlocked, SfxSecretUnlocked},
        };

    }
    public void PlaySfx(SoundEvent eventType)
    {
        if (!_sfxEntries.ContainsKey(eventType))
        {
            Debug.LogWarning("Unrecognised SoundEvent type: " + eventType);
            return;
        }
        _sfxEntries[eventType].PlayUnlessNull();
    }

    public void PlayStarAttainedSfx(int starCount)
    {
        var id = 0;

        if (starCount >= 7)
        {
            id = 4;
        }
        else if (starCount >= 6)
        {
            id = 3;
        }
        else if (starCount >= 5)
        {
            id = 2;
        }
        else if (starCount >= 3)
        {
            id = 1;
        }

        SfxGameplayStars[id].PlayUnlessNull();
    }

    public void PlayEvaluationGradeSfx(int id)
    {
        if (id > SfxEvaluationGrades.Length || id < 0)
        {
            throw new ArgumentException("Invalid ID: " + id);
        }
        SfxEvaluationGrades[id].PlayUnlessNull();
    }
}
