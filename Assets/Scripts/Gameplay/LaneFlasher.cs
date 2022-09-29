using System;
using UnityEngine;

public class LaneFlasher : MonoBehaviour
{
    public const int LANE_COUNT = 3;
    public SpriteFader[] LaneSprites = new SpriteFader[LANE_COUNT];

    private readonly int[] _buttonsHeld = new int[LANE_COUNT];

    public void LaneButtonPressed(int lane)
    {
        _buttonsHeld[lane]++;
    }

    public void LaneButtonReleased(int lane)
    {
        _buttonsHeld[lane] = Math.Max(0, _buttonsHeld[lane] - 1);
    }

    public void FlashLane(int lane)
    {
        LaneSprites[lane].Reset();
    }

    void Update()
    {
        for (int x = 0; x < LANE_COUNT; x++)
        {
            if (_buttonsHeld[x] > 0)
            {
                LaneSprites[x].Reset();
            }
        }
    }

    public void ReleaseAll()
    {
        for (int x = 0; x < LANE_COUNT; x++)
        {
            _buttonsHeld[x] = 0;
        }
    }
}
