using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScroller : MonoBehaviour
{
    public RectTransform OuterContainer;
    public RectTransform InnerContainer;

    public DateTime LastUpdate = DateTime.Now;

    public float ScrollSpeed = 1.0f;
    public float WaitTime = 3.0f;

    [SerializeField]
    private AutoScrollerState _state = AutoScrollerState.NotStarted;

    private float _minY;
    private float _maxY;
    private DateTime _waitUntilTime = DateTime.Now;

    private Vector3[] _corners = new Vector3[4];
    // Update is called once per frame
    void Update()
    {
        UpdateScroll();
        LastUpdate = DateTime.Now;
    }

    private void Reset()
    {
        _state = AutoScrollerState.NotStarted;
        CalculateBounds();

        InnerContainer.localPosition = new Vector3(InnerContainer.localPosition.x, _maxY, InnerContainer.localPosition.z);
    }

    private void CalculateBounds()
    {
        var sizeDelta = Math.Max(0, InnerContainer.rect.height - OuterContainer.rect.height);
        OuterContainer.GetWorldCorners(_corners);
        // MinY is the bottom of the outer container. MaxY is the bottom of the inner container when the top of the inner container is aligned with the top of the outer container.
        _minY = _corners[0].y;
        _maxY = _corners[0].y + sizeDelta;
    }

    private void UpdateScroll()
    {

        CalculateBounds();
        InnerContainer.GetWorldCorners(_corners);
        var currentY = _corners[0].y;
        if (_maxY == 0)
        {
            _state = AutoScrollerState.NotStarted;
            return;
        }

        var positionDelta = (DateTime.Now - LastUpdate).TotalSeconds * ScrollSpeed;
        switch (_state)
        {
            case AutoScrollerState.NotStarted:
                LastUpdate = DateTime.Now;
                _state = AutoScrollerState.ScrollingToEnd;
                break;
            case AutoScrollerState.ScrollingToEnd:
                if (currentY - positionDelta < _minY)
                {
                    positionDelta = currentY - _minY;
                    _state = AutoScrollerState.WaitingAtEnd;
                    _waitUntilTime = DateTime.Now.AddSeconds(WaitTime);
                }
                var newPositionB = new Vector3(InnerContainer.localPosition.x, InnerContainer.localPosition.y - (float)positionDelta, InnerContainer.localPosition.z);
                InnerContainer.localPosition = newPositionB;
                break;
            case AutoScrollerState.ScrollingToStart:
                if (currentY + positionDelta > _maxY)
                {
                    positionDelta = _maxY - currentY;
                    _state = AutoScrollerState.WaitingAtStart;
                    _waitUntilTime = DateTime.Now.AddSeconds(WaitTime);
                }
                var newPositionT = new Vector3(InnerContainer.localPosition.x, InnerContainer.localPosition.y - (float)positionDelta, InnerContainer.localPosition.z);
                InnerContainer.localPosition = newPositionT;
                break;
            case AutoScrollerState.WaitingAtStart:
                if (DateTime.Now > _waitUntilTime)
                {
                    _state = AutoScrollerState.ScrollingToEnd;
                }
                break;
            case AutoScrollerState.WaitingAtEnd:
                if (DateTime.Now > _waitUntilTime)
                {
                    _state = AutoScrollerState.ScrollingToStart;
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
