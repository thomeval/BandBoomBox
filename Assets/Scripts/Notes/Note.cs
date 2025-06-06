﻿using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Note : MonoBehaviour
{
    public NoteBase NoteBase = new();

    public float Position
    {
        get => NoteBase.Position;
        set => NoteBase.Position = value;
    }

    public float AbsoluteTime
    {
        get => NoteBase.AbsoluteTime;
        set => NoteBase.AbsoluteTime = value;
    }

    public NoteType NoteType
    {
        get => NoteBase.NoteType;
        set => NoteBase.NoteType = value;
    }

    public NoteClass NoteClass
    {
        get => NoteBase.NoteClass;
        set => NoteBase.NoteClass = value;
    }

    public int Lane
    {
        get => NoteBase.Lane;
        set => NoteBase.Lane = value;
    }

    public float MxValue
    {
        get => NoteBase.MxValue;
        set => NoteBase.MxValue = value;
    }

    public Note EndNote;

    public GameObject LabelObject;

    public SpriteRenderer HoldTailSprite;
    public int HoldSpriteHeight = 100;

    private SpriteResolver _spriteResolver;
    private SpriteResolver _holdSpriteResolver;
    private SpriteResolver _labelSpriteResolver;

    public string Description => NoteBase.Description;

    void Awake()
    {
        _spriteResolver = GetComponent<SpriteResolver>();
        _labelSpriteResolver = LabelObject.GetComponent<SpriteResolver>();
    }

    public void CalculateTailWidth()
    {
        if (HoldTailSprite == null || EndNote == null)
        {
            return;
        }

        var width = EndNote.transform.localPosition.x - this.transform.localPosition.x;
        width /= this.transform.localScale.x;

        HoldTailSprite.size = new Vector2(width, HoldSpriteHeight);
        HoldTailSprite.transform.localPosition = new Vector3(width / 2, 0, 0);
    }
    public void SetRenderXPosition(float newXPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos, currentPos.y, currentPos.z);
    }

    public void SetRenderYPosition(float newYPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(currentPos.x, newYPos, currentPos.z);
    }

    public void SetRenderPosition(float newXPos, float newYPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos, newYPos, currentPos.z);
    }

    public void SetSpriteCategories(string noteSkinCat, string labelSkinCat)
    {
        if (_spriteResolver == null)
        {
            Awake();
        }

        if (HoldTailSprite != null && _holdSpriteResolver == null)
        {
            _holdSpriteResolver = HoldTailSprite.GetComponent<SpriteResolver>();
        }

        if (this.NoteClass == NoteClass.Release)
        {
            noteSkinCat = "HoldTailEnds";
            LabelObject.SetActive(false);
        }
        else
        {
            noteSkinCat = "Notes_" + noteSkinCat;
            LabelObject.SetActive(true);
        }

        _spriteResolver.SetCategoryAndLabel(noteSkinCat, NoteType.ToString());
        _labelSpriteResolver.SetCategoryAndLabel("Labels_" + labelSkinCat, NoteType.ToString());
        _holdSpriteResolver?.SetCategoryAndLabel("HoldTails", NoteType.ToString());
    }

    public void Refresh()
    {
        RefreshSprites();
        RefreshLane();
    }

    public void RefreshSprites()
    {
        if (_spriteResolver == null)
        {
            Awake();
        }

        if (HoldTailSprite != null && _holdSpriteResolver == null)
        {
            _holdSpriteResolver = HoldTailSprite.GetComponent<SpriteResolver>();
        }

        _spriteResolver.SetCategoryAndLabel(_spriteResolver.GetCategory(), NoteType.ToString());
        _labelSpriteResolver.SetCategoryAndLabel(_labelSpriteResolver.GetCategory(), NoteType.ToString());
        _holdSpriteResolver?.SetCategoryAndLabel(_holdSpriteResolver.GetCategory(), NoteType.ToString());
    }

    public void RefreshLane()
    {
        this.Lane = NoteUtils.GetNoteLane(this.NoteType);
    }

    public float DistanceTo(float absoluteTime)
    {
        return MathF.Abs(absoluteTime - this.AbsoluteTime);
    }

    public bool IsEndNote { get { return this.NoteClass == NoteClass.Release; } }
}

