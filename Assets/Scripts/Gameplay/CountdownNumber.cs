using UnityEngine;
using UnityEngine.U2D.Animation;

public class CountdownNumber : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private SpriteResolver _spriteResolver;

    void Awake()
    {
        _spriteResolver = GetComponent<SpriteResolver>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DisplayBeat(float songTimeInBeats)
    {
        if (songTimeInBeats > 0.0f || songTimeInBeats < -4.0f)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(true);
        songTimeInBeats = Mathf.Abs(songTimeInBeats);
        var beat = (int) songTimeInBeats ;
        var fraction = songTimeInBeats - beat;

        Debug.Assert(beat >= 0 && beat <= 3);
        _spriteResolver.SetCategoryAndLabel("Countdowns", "" + (beat+1));
        _spriteRenderer.color = new Color(1.0f,1.0f,1.0f,fraction);
    }
}
