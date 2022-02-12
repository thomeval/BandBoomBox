using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BeatLine : MonoBehaviour
{
    public float Position;
    public float AbsoluteTime;

    public BeatLineType BeatLineType;
    public int XPositionOffset = 0;
    private SpriteResolver _spriteResolver;

    void Awake()
    {
        _spriteResolver = GetComponent<SpriteResolver>();
    }

    public void SetPosition(float newXPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos + XPositionOffset, currentPos.y, currentPos.z);
    }

    public void SetPosition(float newXPos, float newYPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos + XPositionOffset, newYPos, currentPos.z);
    }

    public void SetSprite()
    {
        _spriteResolver.SetCategoryAndLabel("BeatLines", BeatLineType.ToString());
    }
}
