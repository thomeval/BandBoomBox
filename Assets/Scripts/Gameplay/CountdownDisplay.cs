using UnityEngine;
using UnityEngine.UI;

public class CountdownDisplay : MonoBehaviour
{
    public GameObject ReadySprite;
    public CountdownNumber CountdownNumber;
    public Text PlayerNameText;

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

        var opacity = -0.25f * songTimeInBeats;
        opacity = Mathf.Clamp(opacity, 0.0f, 1.0f);
        PlayerNameText.color = new Color(1.0f, 1.0f, 1.0f, opacity);
    }
}
