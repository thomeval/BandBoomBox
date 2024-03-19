using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnergyMeter : MonoBehaviour
{
    public SpriteMeter SpriteMeter;
    public SpriteResolver SpriteResolver;
    public EnergyMeterScaleManager ScaleManager;

    private string _lastSprite = "Inactive";

    [SerializeField]
    private double _energy;

    public double Energy
    {
        get { return _energy; }
        set
        {
            _energy = Math.Clamp(value, 0.0f, MaxEnergy);
            SpriteMeter.Value = (float)(_energy / MaxEnergy);
            SetSprite();
        }
    }

    public double MaxEnergy = 1.0;

    [SerializeField]
    private bool _turboActive;

    public bool TurboActive
    {
        get { return _turboActive; }
        set
        {
            _turboActive = value;
            SetSprite();
        }
    }

    private void SetSprite()
    {
        var spriteLabel = (Energy >= MaxEnergy) ? "Full" :
            _turboActive ? "Active" : "Inactive";

        if (_lastSprite == spriteLabel)
        {
            return;
        }

        SpriteResolver.SetCategoryAndLabel("EnergyMeter", spriteLabel);
        _lastSprite = spriteLabel;
    }

    public void SetMaxEnergy(double maxEnergy)
    {
        if (maxEnergy == MaxEnergy)
        {
            return;
        }

        MaxEnergy = maxEnergy;
        ScaleManager.DrawScaleLines();
    }
}

