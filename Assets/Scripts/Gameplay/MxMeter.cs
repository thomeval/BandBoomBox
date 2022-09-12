using System;
using UnityEngine;
using UnityEngine.UI;

public class MxMeter : MonoBehaviour
{
    public GameObject FrontSprite;
    public GameObject FrontSpriteMask;
    public GameObject BackSprite;
    public GameObject GlowSprite;

    public Text TextWhole;
    public Text TextFraction;

    private SpriteRenderer _frontRend;
    private SpriteRenderer _backRend;
    private SpriteRenderer _glowRend;

    private readonly Color[] _colors =
    {
        new Color(0.2f, 0.2f, 1.0f),
        new Color(0.6f, 0.6f, 1.0f),
        new Color(0.2f, 0.6f, 0.6f),
        new Color(0.2f, 1.0f, 1.0f),
        new Color(0.2f, 1.0f, 0.2f),
        new Color(0.6f, 1.0f, 0.2f),
        new Color(1.0f, 1.0f, 0.2f),
        new Color(1.0f, 0.7f, 0.2f),
        new Color(1.0f, 0.4f, 0.2f),
        new Color(1.0f, 0.2f, 0.2f),
        new Color(1.0f, 0.2f, 0.6f),
        new Color(0.8f, 0.2f, 0.8f),
        new Color(0.4f, 0.2f, 0.8f)
    };

    public int MeterHeight = 980;
    public int MeterWidth = 260;

    [SerializeField]
    private float _multiplier;
    public float Multiplier
    {
        get { return _multiplier; }
        set
        {
            _multiplier = value;
            UpdateSprites();
        }
    }

    public float BeatFraction;

    void Awake()
    {
        _frontRend = FrontSprite.GetComponent<SpriteRenderer>();
        _backRend = BackSprite.GetComponent<SpriteRenderer>();
        _glowRend = GlowSprite.GetComponent<SpriteRenderer>();
    }

    private void UpdateSprites()
    {

        var whole = (int)_multiplier;
        var fraction = _multiplier - whole;

        UpdateColors(whole);
        UpdateFrontSpriteMask(fraction);
        UpdateGlow();

        TextWhole.text = string.Format("{0:00}.", whole);
        TextFraction.text = string.Format("{0:00}", fraction * 100);
    }

    private void UpdateGlow()
    {
        var badFraction = 1.0f - Multiplier;
        var goodFraction = Math.Min(1.0f, (Multiplier - 1) / 20);
        goodFraction *= (1-BeatFraction);
        var color = Multiplier < 1.0f
            ? new Color(1.0f, 0.0f, 0.0f, badFraction)
            : new Color(1.0f, 1.0f, 1.0f, goodFraction);

        _glowRend.color = color;
    }

    private void UpdateFrontSpriteMask(float fraction)
    {
        if (_multiplier < 1.0f)
        {
            fraction = 0.0f;
        }

        var currentHeight = MeterHeight * fraction;
        var middle = MeterHeight / -2.0f; // Set to bottom edge
        middle += currentHeight / 2; // Shift upward
        FrontSpriteMask.transform.localScale = new Vector3(MeterWidth, fraction * MeterHeight);
        FrontSpriteMask.transform.localPosition = new Vector3(0, middle, 0);
    }
    private void UpdateColors(int whole)
    {
        if (whole < 2)
        {
            _backRend.color = new Color(0.0f,0.0f,0.0f,0.0f);
        }
        else
        {
            _backRend.color = _colors[(whole - 2) % _colors.Length];
        }

        if (whole >= 1)
        {
            _frontRend.color = _colors[(whole - 1) % _colors.Length];
        }
    }
}
