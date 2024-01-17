using System.Collections.Generic;
using UnityEngine;

public class MenuSoundEventHandler : MonoBehaviour
{
    [Header("Sounds")]
    [Header("General")]
    public AudioSource SfxMistake;

    [Header("Menus")]
    public AudioSource SfxSelectionChanged;
    public AudioSource SfxSelectionConfirmed;
    public AudioSource SfxSelectionCancelled;
    public AudioSource SfxSelectionShifted;

    protected Dictionary<SoundEvent, AudioSource> _sfxEntries;

    void Awake()
    {
        SetupSfxEntries();
    }

    protected virtual void SetupSfxEntries()
    {
        _sfxEntries = new Dictionary<SoundEvent, AudioSource>()
        {
            { SoundEvent.Mistake, SfxMistake},
            { SoundEvent.SelectionChanged, SfxSelectionChanged},
            { SoundEvent.SelectionConfirmed, SfxSelectionConfirmed},
            { SoundEvent.SelectionCancelled, SfxSelectionCancelled},
            { SoundEvent.SelectionShifted, SfxSelectionShifted},
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

}
