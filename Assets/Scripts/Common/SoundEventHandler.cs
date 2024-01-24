using System;
using UnityEngine;

public class SoundEventHandler : MenuSoundEventHandler
{
    [Header("Sounds")]

    [Header("Gameplay")]
    public AudioSource SfxGameplayTurboOff;
    public AudioSource SfxGameplayTurboOn;
    public AudioSource[] SfxGameplayStars = new AudioSource[5];

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

    protected override void SetupSfxEntries()
    {
        base.SetupSfxEntries();

        _sfxEntries.Add(SoundEvent.Editor_SaveComplete, SfxEditorSaveComplete);
        _sfxEntries.Add(SoundEvent.Editor_NotePlaced, SfxEditorNotePlaced);
        _sfxEntries.Add(SoundEvent.Editor_NoteRemoved, SfxEditorNoteRemoved);

        _sfxEntries.Add(SoundEvent.Editor_Cut, SfxEditorCut);
        _sfxEntries.Add(SoundEvent.Editor_Copy, SfxEditorCopy);
        _sfxEntries.Add(SoundEvent.Editor_Paste, SfxEditorPaste);
        _sfxEntries.Add(SoundEvent.Editor_SelectRegionStart, SfxEditorSelectRegionStart);
        _sfxEntries.Add(SoundEvent.Editor_SelectRegionEnd, SfxEditorSelectRegionEnd);

        _sfxEntries.Add(SoundEvent.Gameplay_TurboOff, SfxGameplayTurboOff);
        _sfxEntries.Add(SoundEvent.Gameplay_TurboOn, SfxGameplayTurboOn);

        _sfxEntries.Add(SoundEvent.TitleScreen_StartPressed, SfxTitleScreenStartPressed);
        _sfxEntries.Add(SoundEvent.SecretUnlocked, SfxSecretUnlocked);
    }

    void Awake()
    {
        SetupSfxEntries();
    }

    public void PlayStarAttainedSfx(int starCount)
    {
        if (starCount <= 0)
        {
            return;
        }

        var id = 0;

        if (starCount >= 9)
        {
            id = 6;
        }
        else if (starCount >= 8)
        {
            id = 5;
        }
        else if (starCount >= 7)
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
