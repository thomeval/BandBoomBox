using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    private readonly List<MenuMusicEntry> _menuMusicItems = new List<MenuMusicEntry>();

    public GameObject MenuMusicEntryCollection;

    public AudioCrossFader AudioCrossFader;

    public MenuMusicEntry CurrentMusic;

    public string LastGroup;

    void Awake()
    {
        if (AudioCrossFader == null)
        {
            AudioCrossFader = GetComponent<AudioCrossFader>();
        }

        var entries = MenuMusicEntryCollection.GetComponentsInChildren<MenuMusicEntry>();
        _menuMusicItems.AddRange(entries);
    }

    public void PlaySceneMusic(GameScene gameScene)
    {
        var entry = _menuMusicItems.FirstOrDefault(e => e.GameScene == gameScene);
        CurrentMusic = entry;

        var clip = CurrentMusic?.AudioClip;
        AudioCrossFader.Play(clip, LastGroup == entry?.Group);

        LastGroup = CurrentMusic?.Group;
    }

    public void StopAll()
    {
        AudioCrossFader.StopAllImmediate();
    }
}

