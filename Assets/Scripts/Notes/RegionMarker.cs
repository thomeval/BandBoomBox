using UnityEngine;

public class RegionMarker : MonoBehaviour
{
    public float StartPosition;
    public float StartAbsoluteTime;
    public float EndPosition;
    public float EndAbsoluteTime;

    private SpriteRenderer _spriteRenderer;
    public void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float MiddleAbsoluteTime
    {
        get
        {
            return (EndAbsoluteTime + StartAbsoluteTime) / 2;
        }
    }

    public RegionMarkerType RegionMarkerType;

    public void SetRenderPosition(float newXPosStart, float newXPosEnd)
    {
        var currentPos = this.transform.localPosition;
        var middleX = (newXPosEnd + newXPosStart) / 2;

        this._spriteRenderer.size = new Vector2(newXPosEnd - newXPosStart, this._spriteRenderer.size.y);
        this.transform.localPosition = new Vector3(middleX, currentPos.y, currentPos.z);
    }

    public void SetRenderPosition(float newXPosStart, float newXPosEnd, float newYPosStart, float newYPosEnd)
    {
        var currentPos = this.transform.localPosition;
        var middleX = (newXPosEnd + newXPosStart) / 2;
        var middleY = (newYPosEnd + newYPosStart) / 2;

        this._spriteRenderer.size = new Vector2(newXPosEnd - newXPosStart, newYPosEnd - newYPosStart);
        this.transform.localPosition = new Vector3(middleX, middleY, currentPos.z);
    }
}
