using UnityEngine;


public class MenuMusicEntry : MonoBehaviour
{
    public GameScene GameScene;

    public AudioClip AudioClip;

    public string Group;

    // TODO: Should be nullable, but this breaks Unity's inspector.
    public float Bpm;

}
