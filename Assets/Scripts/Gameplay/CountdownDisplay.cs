using UnityEngine;
using UnityEngine.UI;

public class CountdownDisplay : MonoBehaviour
{
    public GameObject ReadySprite;
    public CountdownNumber CountdownNumber;
    public Text PlayerNameText;
    public HighwayNameDisplay HighwayNameDisplay;

    private float _lastOpacity = -1.0f;

    public string PlayerName
    {
        get
        {
            return PlayerNameText.text;
        }
        set
        {
            PlayerNameText.text = value;
        }
    }

    public void DisplayBeat(float songTimeInBeats)
    {
        if (songTimeInBeats > 0.0f)
        {
            return;
        }

        DisplayPlayerName(songTimeInBeats);

        ReadySprite.SetActive(songTimeInBeats < -4.0f);
        CountdownNumber.DisplayBeat(songTimeInBeats);
    }

    private void DisplayPlayerName(float songTimeInBeats)
    {
        var opacity = 0.0f;

        switch (HighwayNameDisplay)
        {
            case HighwayNameDisplay.Never:
                break;
            case HighwayNameDisplay.Always:
                opacity = 1.0f;
                break;
            case HighwayNameDisplay.SongStart:
                opacity = -0.25f * songTimeInBeats;
                opacity = Mathf.Clamp(opacity, 0.0f, 1.0f);
                break;
        }

        if (Mathf.Approximately(_lastOpacity, opacity))
        {
            return; // No change in opacity, no need to update
        }

        _lastOpacity = opacity;
        PlayerNameText.color = new Color(1.0f, 1.0f, 1.0f, opacity);
    }
}
