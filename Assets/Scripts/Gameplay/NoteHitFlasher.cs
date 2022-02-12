using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHitFlasher : MonoBehaviour
{
    public AnimatedSprite[] LaneSprites;

    void Start()
    {
        Reset();
    }
    public void Reset()
    {
        foreach (var laneSprite in LaneSprites)
        {
            laneSprite.SkipToLastFrame();
        }
    }

    public void FlashNoteHit(int lane)
    {
        LaneSprites[lane].Play();
    }
}
