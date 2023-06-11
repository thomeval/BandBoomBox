using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnergyMeter : MonoBehaviour
{

    public SpriteMeter SpriteMeter;
    public SpriteResolver SpriteResolver;

    private string _lastSprite = "Inactive";

    [SerializeField]
    private float _energy;

    public float Energy
    {
        get { return _energy; }
        set
        {
            _energy = Mathf.Clamp(value,0.0f, MaxEnergy);
            SpriteMeter.Value = _energy / MaxEnergy;
            SetSprite();
        }
    }

    public float MaxEnergy = 1.0f;

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
}

