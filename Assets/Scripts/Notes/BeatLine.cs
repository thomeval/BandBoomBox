using UnityEngine;

public class BeatLine : MonoBehaviour
{
    public float Position;
    public float AbsoluteTime;

    public BeatLineType BeatLineType;
    public int XPositionOffset = 0;


    public void SetRenderPosition(float newXPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos + XPositionOffset, currentPos.y, currentPos.z);
    }

    public void SetRenderPosition(float newXPos, float newYPos)
    {
        var currentPos = this.transform.localPosition;
        this.transform.localPosition = new Vector3(newXPos + XPositionOffset, newYPos, currentPos.z);
    }
}
