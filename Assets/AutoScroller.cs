using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroller : MonoBehaviour
{
    public RectTransform OuterContainer;
    public RectTransform InnerContainer;


    public DateTime LastUpdate = DateTime.Now;

    /// <summary>
    /// The speed at which to scroll InnerContainer, in units per second.
    /// </summary>
    public float ScrollSpeed = 1.0f;

    /// <summary>
    /// The amount of time to pause scrolling when at the top or bottom, in seconds.
    /// </summary>
    public float WaitTime = 3.0f;

    [SerializeField]
    private AutoScrollerState _state = AutoScrollerState.NotStarted;

    void Update()
    {
        UpdateScroll();
        LastUpdate = DateTime.Now;
    }

    private float _waitTimer = 0f;

    public void Reset()
    {
        this.enabled = true;
        _state = AutoScrollerState.NotStarted;
        _waitTimer = 0f;

        if (OuterContainer != null && InnerContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(InnerContainer);
            Vector3 localPos = InnerContainer.localPosition;
            localPos.y = OuterContainer.rect.yMax - InnerContainer.rect.yMax;
            InnerContainer.localPosition = localPos;
        }
    }

    private void UpdateScroll()
    {
        if (OuterContainer == null || InnerContainer == null)
        {
            return;
        }

        if (InnerContainer.rect.height <= OuterContainer.rect.height)
        {
            Reset();
            this.enabled = false;
            return;
        }

        float dt = (float)(DateTime.Now - LastUpdate).TotalSeconds;

        if (_state == AutoScrollerState.NotStarted)
        {
            Reset();
            _state = AutoScrollerState.WaitingAtStart;
        }

        switch (_state)
        {
            case AutoScrollerState.WaitingAtStart:
                _waitTimer += dt;
                if (_waitTimer >= WaitTime)
                {
                    _waitTimer = 0f;
                    _state = AutoScrollerState.ScrollingToEnd;
                }
                break;

            case AutoScrollerState.ScrollingToEnd:
                InnerContainer.localPosition += new Vector3(0, ScrollSpeed * dt, 0);

                if (InnerContainer.localPosition.y + InnerContainer.rect.yMin >= OuterContainer.rect.yMin)
                {
                    Vector3 localPos = InnerContainer.localPosition;
                    localPos.y = OuterContainer.rect.yMin - InnerContainer.rect.yMin;
                    InnerContainer.localPosition = localPos;
                    _state = AutoScrollerState.WaitingAtEnd;
                }
                break;

            case AutoScrollerState.WaitingAtEnd:
                _waitTimer += dt;
                if (_waitTimer >= WaitTime)
                {
                    _waitTimer = 0f;
                    _state = AutoScrollerState.ScrollingToStart;
                }
                break;

            case AutoScrollerState.ScrollingToStart:
                InnerContainer.localPosition -= new Vector3(0, ScrollSpeed * dt, 0);

                if (InnerContainer.localPosition.y + InnerContainer.rect.yMax <= OuterContainer.rect.yMax)
                {
                    Vector3 localPos = InnerContainer.localPosition;
                    localPos.y = OuterContainer.rect.yMax - InnerContainer.rect.yMax;
                    InnerContainer.localPosition = localPos;
                    _state = AutoScrollerState.WaitingAtStart;
                }
                break;
        }
    }
}

public enum AutoScrollerState
{
    NotStarted,
    ScrollingToStart,
    ScrollingToEnd,
    WaitingAtStart,
    WaitingAtEnd
}
