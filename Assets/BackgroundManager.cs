using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public bool AutoUpdateBeatNumber = true;
    
    public BackgroundPlaneAnimator BackgroundPlaneAnimator;

    public int BeatCycle = 64;

    [SerializeField]
    private double _beatNumber;
    public double BeatNumber
    {
        get
        {
            return _beatNumber;
        }
        private set
        {
            _beatNumber = value;
            BackgroundPlaneAnimator.BeatNumber = value;
        }
    }

    public float Bpm = 120.0f;

    private DateTime _lastUpdate;
    
    void Awake()
    {
        DontDestroyOnLoad(this);
        _lastUpdate = DateTime.Now;
    }

    void FixedUpdate()
    {
        var deltaSeconds = (DateTime.Now - _lastUpdate).TotalSeconds;
        UpdateBeatNumber(deltaSeconds);
        _lastUpdate = DateTime.Now;
    }

    public void SetBpm(float bpm, bool autoUpdateBeatNumber)
    {
        this.AutoUpdateBeatNumber = autoUpdateBeatNumber;
        this.Bpm = bpm;
        this.BeatNumber = (int) this.BeatNumber;
    }

    public void SetBeatNumber(double beatNumber)
    {
        var fraction = beatNumber - (int)beatNumber;
        var integer = (int)beatNumber;
        integer %= BeatCycle;

        this.BeatNumber = integer + fraction;
    }

    public void SetSpeedMultiplier(float speed)
    {
        BackgroundPlaneAnimator.SpeedMultiplier = speed;
    }

    private void UpdateBeatNumber(double deltaSeconds)
    {
        if (!AutoUpdateBeatNumber)
        {
            return;
        }
        
        BeatNumber += deltaSeconds / 60 * Bpm;

        if (BeatNumber > BeatCycle)
        {
            BeatNumber -= BeatCycle;
        }
    }
}
