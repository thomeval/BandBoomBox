using System;
using UnityEngine;

public class SpriteFader : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float FadeTime = 1.0f;

    private DateTime _lastUpdate;

    public float Opacity;
    public Color SpriteColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        _lastUpdate = DateTime.Now;
        SpriteRenderer.color = new Color(SpriteColor.r, SpriteColor.g, SpriteColor.b, Opacity);
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeTime == 0.0f || Opacity == 0.0f)
        {
            return;
        }

        var timeDiff = (float)(DateTime.Now - _lastUpdate).TotalSeconds;
        _lastUpdate = DateTime.Now;



        var fadeAmount = 1.0f / FadeTime;
        var opacityDelta = fadeAmount * timeDiff;

        Opacity = Mathf.Max(0.0f, Opacity - opacityDelta);
        SpriteRenderer.color = new Color(SpriteColor.r,SpriteColor.g,SpriteColor.b, Opacity);
    }

    public void Reset()
    {
        Opacity = 1.0f;
        _lastUpdate = DateTime.Now;
    }
}
