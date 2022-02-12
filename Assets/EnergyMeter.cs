using UnityEngine;
using UnityEngine.U2D.Animation;

public class EnergyMeter : MonoBehaviour
{

    public SpriteMeter SpriteMeter;
    public SpriteResolver SpriteResolver;

    [SerializeField]
    private float _energy;

    public float Energy
    {
        get { return _energy; }
        set
        {
            _energy = Mathf.Clamp(value,0.0f, MaxEnergy);
            SpriteMeter.Value = _energy / MaxEnergy;
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
            var spriteLabel = _turboActive ? "Active" : "Inactive";
            SpriteResolver.SetCategoryAndLabel("EnergyMeter", spriteLabel);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
