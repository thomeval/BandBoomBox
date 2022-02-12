using System;
using UnityEngine;

public class AudioCrossFader : MonoBehaviour
{
    [SerializeField] public AudioSource[] Tracks;

    [SerializeField]
    private string[] _trackNames;

    [SerializeField]
    private int _activeTrack;

    public float CrossFadeTime = 2.0f;

    public AudioSource CurrentAudioSource
    {
        get
        {
            if (_activeTrack == -1)
            {
                return null;
            }

            return Tracks[_activeTrack];
        }
    }

    public string CurrentTrackName
    {
        get
        {
            if (_activeTrack == -1)
            {
                return null;
            }
            return _trackNames[_activeTrack];
        }
    }

    void Awake()
    {
        _trackNames = new string[Tracks.Length];
    }

    public void Play(AudioClip clip, bool syncWithLastClip)
    {
        if (clip == null)
        {
            _activeTrack = -1;
            return;
        }

        var existingTrack = GetTrackByAudioName(clip.name);

        var pos = GetCurrentTrackTimeOrDefault(syncWithLastClip);

        // Reuse an existing track. If its stopped, start playing it again.
        if (existingTrack != -1)
        {
            
            _activeTrack = existingTrack;
            if (!CurrentAudioSource.isPlaying)
            {
                Play(Tracks[existingTrack].clip, existingTrack, pos);
            }
        }

        // A new audio clip. Slot it into an available track.
        else
        {
            _activeTrack = (_activeTrack + 1) % Tracks.Length;
            Play(clip, _activeTrack, pos);
        }
    }

    private float GetCurrentTrackTimeOrDefault(bool syncWithLastClip)
    {
        if (!syncWithLastClip)
        {
            return 0.0f;
        }

        if (_activeTrack == -1)
        {
            return 0.0f;
        }
        return Tracks[_activeTrack]?.time ?? 0.0f;
    }

    public void Play(AudioClip clip)
    {
        Play(clip, false);
    }

    private int GetTrackByAudioName(string clipName)
    {
        return Array.IndexOf(_trackNames, clipName);
    }

    public void Play(AudioClip clip, int track, float pos)
    {
        Tracks[track].clip = clip;
        Tracks[track].Play();
        _trackNames[track] = clip.name;
        Tracks[_activeTrack].time = pos;

    }

    public void StopAllImmediate()
    {
        for (int x = 0; x < Tracks.Length; x++)
        {
            Tracks[x].volume = 0.0f;
            _trackNames[x] = null;
            Tracks[x].Stop();
        }
    }

    void FixedUpdate()
    {
        if (CrossFadeTime <= 0.0f)
        {
            return;
        }

        var delta = Time.fixedDeltaTime / CrossFadeTime;

        foreach (var track in Tracks)
        {
            if (track == CurrentAudioSource)
            {
                continue;
            }

            AdjustVolume(track, -delta);
            if (track.volume <= 0.0f)
            {
                track.Stop();
            }
        }

        if (CurrentAudioSource != null)
        {
            AdjustVolume(CurrentAudioSource, delta * 2);
        }
    }

    private void AdjustVolume(AudioSource audioSource, float delta)
    {
        audioSource.volume = Mathf.Clamp(audioSource.volume + delta, 0.0f, 1.0f);
    }

}
