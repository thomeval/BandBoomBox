using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class AnimatedSprite : MonoBehaviour
{

    [SerializeField]
    private int _currentFrame;

    public int CurrentFrame
    {
        get { return _currentFrame; }
        private set
        {
            _currentFrame = value;
            SetSpriteFrame();
        }
    }

    public int TotalFrames;
    public float TimePerFrame = 0.5f;
    public bool PlayOnAwake;
    private DateTime _startTime;
    private SpriteResolver _spriteResolver;

    public bool IsPlaying
    {
        get { return this.enabled; }
    }

    void Awake()
    {
        _spriteResolver = GetComponent<SpriteResolver>();
        this.enabled = this.PlayOnAwake;
        _startTime = DateTime.Now;
        CurrentFrame = 0;
    }

    public void Play()
    {
        this.enabled = true;
        _startTime = DateTime.Now;
        CurrentFrame = 0;
        SetSpriteFrame();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimePerFrame == 0.0f)
        {
            return;
        }
        var timeSinceStart = DateTime.Now.Subtract(_startTime).TotalSeconds;
        var frameNum = (int) (timeSinceStart / TimePerFrame);

        CurrentFrame = frameNum;

        if (CurrentFrame > TotalFrames)
        {
            this.enabled = false;
            return;
        }
        SetSpriteFrame();
    }

    private void SetSpriteFrame()
    {
        var actualFrame = Math.Min(CurrentFrame, TotalFrames - 1);
        _spriteResolver.SetCategoryAndLabel(_spriteResolver.GetCategory(), "" + actualFrame);
    }

    public void SkipToLastFrame()
    {
        CurrentFrame = this.TotalFrames - 1;
        _startTime = DateTime.Now.AddSeconds(CurrentFrame * TimePerFrame);
    }
}
