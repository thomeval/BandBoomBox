using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class StarMeter : MonoBehaviour
{
    public SpriteResolver StarSpriteResolver;
    public SpriteMeter ProgressBar;

    public bool ProgressBarVisible;

    private double _lastValue = 0.0;

    [SerializeField]
    private double _value;
    public double Value
    {
        get { return _value; }
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_value == value)
            {
                return;
            }

            _value = value;

            // Display correct number of stars
            int stars = (int) value;
            StarSpriteResolver.SetCategoryAndLabel("Stars", "" + stars);

            // Update the progress bar
            float fraction = (float) value - stars;
            ProgressBar.Value = fraction;

            // Play star Sfx if appropriate
            PlaySfx();
            _lastValue = value;
        }
    }

    [Header("Sounds")]
    public bool SfxEnabled;

    public SoundEventProvider SoundEventProvider;

    private void PlaySfx()
    {
        if (!SfxEnabled)
        {
            return;
        }
        var lastStar = (int) _lastValue;
        var curStar = (int) _value;

        if (curStar > lastStar)
        {
            SoundEventProvider.PlayStarAttainedSfx(curStar);
        }
    }

    void Awake()
    {
        Helpers.AutoAssign(ref SoundEventProvider);        
    }

    void Start()
    {
        ProgressBar.gameObject.SetActive(ProgressBarVisible);
    }

}
