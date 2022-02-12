using UnityEngine;

public class CountdownDisplay : MonoBehaviour
{
    public GameObject ReadySprite;
    public CountdownNumber CountdownNumber;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayBeat(float songTimeInBeats)
    {
        ReadySprite.SetActive(songTimeInBeats < -4.0f);

        CountdownNumber.DisplayBeat(songTimeInBeats);

    }
}
