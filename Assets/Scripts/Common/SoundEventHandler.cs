using UnityEngine;

public class SoundEventHandler : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource SfxMistake;
    public AudioSource SfxSelectionConfirmed; 
    public AudioSource SfxSelectionCancelled;
    public AudioSource SfxSelectionShifted;
    public AudioSource SfxEditorSaveComplete;
    public AudioSource SfxEditorNotePlaced;
    public AudioSource SfxEditorNoteRemoved;
    public AudioSource SfxEditorCut;
    public AudioSource SfxEditorCopy;
    public AudioSource SfxEditorPaste;
    public AudioSource SfxGameplayStar1;
    public AudioSource SfxGameplayStar2;
    public AudioSource SfxGameplayStar3;
    public AudioSource SfxTitleScreenStartPressed;
    public AudioSource SfxEditorSelectRegionStart;
    public AudioSource SfxEditorSelectRegionEnd;

    public void PlaySfx(SoundEvent eventType)
    {

        switch (eventType)
        {
            case SoundEvent.Mistake:
                SfxMistake.PlayUnlessNull();
                break;
            case SoundEvent.SelectionConfirmed:
                SfxSelectionConfirmed.PlayUnlessNull();
                break;
            case SoundEvent.SelectionCancelled:
                SfxSelectionCancelled.PlayUnlessNull();
                break;
            case SoundEvent.SelectionShifted:
                SfxSelectionShifted.PlayUnlessNull();
                break;
            case SoundEvent.Editor_SaveComplete:
                SfxEditorSaveComplete.PlayUnlessNull();
                break;
            case SoundEvent.Editor_NotePlaced:
                SfxEditorNotePlaced.PlayUnlessNull();
                break;
            case SoundEvent.Editor_NoteRemoved:
                SfxEditorNoteRemoved.PlayUnlessNull();
                break;
            case SoundEvent.Gameplay_Star1:
                SfxGameplayStar1.PlayUnlessNull();
                break;
            case SoundEvent.Gameplay_Star2:
                SfxGameplayStar2.PlayUnlessNull();
                break;
            case SoundEvent.Gameplay_Star3:
                SfxGameplayStar3.PlayUnlessNull();
                break;
            case SoundEvent.Editor_Cut:
                SfxEditorCut.PlayUnlessNull();
                break;
            case SoundEvent.Editor_Copy:
                SfxEditorCopy.PlayUnlessNull();
                break;
            case SoundEvent.Editor_Paste:
                SfxEditorPaste.PlayUnlessNull();
                break;
            case SoundEvent.TitleScreen_StartPressed:
                SfxTitleScreenStartPressed.PlayUnlessNull();
                break;
            case SoundEvent.Editor_SelectRegionStart:
                SfxEditorSelectRegionStart.PlayUnlessNull();
                break;
            case SoundEvent.Editor_SelectRegionEnd:
                SfxEditorSelectRegionEnd.PlayUnlessNull();
                break;
            default:
                Debug.LogWarning("Unrecognised SoundEvent type: " + eventType);
                break;
        }
    }

    public void PlayStarAttainedSfx(int starCount)
    {
        if (starCount >= 5)
        {
            PlaySfx(SoundEvent.Gameplay_Star3);
            return;
        }

        if (starCount >= 3)
        {
            PlaySfx(SoundEvent.Gameplay_Star2);
            return;
        }

        PlaySfx(SoundEvent.Gameplay_Star1);
    }
}
