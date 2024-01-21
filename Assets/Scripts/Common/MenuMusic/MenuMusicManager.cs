using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class MenuMusicManager : MonoBehaviour
{
    private readonly List<MenuMusicEntry> _menuMusicItems = new List<MenuMusicEntry>();

    public GameObject MenuMusicEntryCollection;

    public AudioCrossFader AudioCrossFader;
    public BackgroundManager BackgroundManager;

    public MenuMusicEntry CurrentMusic;

    public string LastGroup;

    void Awake()
    {
        if (AudioCrossFader == null)
        {
            AudioCrossFader = GetComponent<AudioCrossFader>();
        }

        if (BackgroundManager == null)
        {
            BackgroundManager = FindObjectOfType<BackgroundManager>();
        }

        var entries = MenuMusicEntryCollection.GetComponentsInChildren<MenuMusicEntry>();
        _menuMusicItems.AddRange(entries);
    }

    public void PlaySceneMusic(GameScene gameScene)
    {
        var entry = _menuMusicItems.FirstOrDefault(e => e.GameScene == gameScene);

        if (entry == null)
        {
            entry = _menuMusicItems.FirstOrDefault(e => e.GameScene == GameScene.All);
        }

        if (entry.Group != CurrentMusic?.Group)
        {
            ApplyToBackground(entry);
        }
        CurrentMusic = entry;

        var clip = CurrentMusic?.AudioClip;
        AudioCrossFader.Play(clip, LastGroup == entry?.Group);

        LastGroup = CurrentMusic?.Group;
    }

    private void ApplyToBackground(MenuMusicEntry entry)
    {
        var hasBpmValue = entry.Bpm >= 0.0f;
        var bpm = hasBpmValue ? entry.Bpm : 0.0f;
        BackgroundManager.SetBpm(bpm, hasBpmValue);
        BackgroundManager.SetSpeedMultiplier(1.0f);
    }

    public void StopAll()
    {
        AudioCrossFader.StopAllImmediate();
    }
}

