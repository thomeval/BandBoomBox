using System;
using UnityEngine;

public class SoundEventProvider : MonoBehaviour
{
    public MenuSoundEventHandler[] MenuSoundEventHandlers = new MenuSoundEventHandler[6];
    public SoundEventHandler GeneralSoundEventHandler;

    public void PlaySfx(SoundEvent soundEvent, int playerSlot)
    {
        if (playerSlot == 0)
        {
            GeneralSoundEventHandler.PlaySfx(soundEvent);
            return;
        }

        if (playerSlot > MenuSoundEventHandlers.Length)
        {
            Debug.LogWarning($"Player slot {playerSlot} is out of range for MenuSoundEventHandlers. Max is {MenuSoundEventHandlers.Length}.");
            return;
        }

        var handler = MenuSoundEventHandlers[playerSlot - 1];
        handler.PlaySfx(soundEvent);
    }

    public void PlayStarAttainedSfx(int starCount)
    {
        GeneralSoundEventHandler.PlayStarAttainedSfx(starCount);
    }

    public void PlayEvaluationGradeSfx(int sfxId)
    {
        GeneralSoundEventHandler.PlayEvaluationGradeSfx(sfxId);
    }
}
