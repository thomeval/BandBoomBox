using UnityEngine;

public class BeatFractionProvider : MonoBehaviour
{
    private SongManager _songManager;
    public float CurrentBeatFraction
    {
        get
        {
            if (_songManager == null)
            {
                return 0;
            }

            var beat = _songManager.GetSongPositionInBeats();
            return beat - (int)beat;
        }
    }

    public float InverseBeatFraction
    {
        get
        {
            return 1 - CurrentBeatFraction;
        }
    }

    private void Awake()
    {
        Helpers.AutoAssign(ref _songManager);
    }
}
