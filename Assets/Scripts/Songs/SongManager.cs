using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{
    public SongData CurrentSong { get; set; }

    [field:SerializeField]
    public AudioChannelFader GameplayMusicFader { get;set; }

    public bool IsSongPlaying
    {
        get { return _audioSource.isPlaying; }
    }

    /// <summary>
    /// Returns the audio latency of Unity's internal sound engine, in seconds.
    /// </summary>
    public float EngineOffset = 0.025f;
    private AudioSource _audioSource;

    /// <summary>
    /// Returns the audio latency setting set by the user, in seconds.
    /// </summary>
    public float UserAudioLatency
    {
        get { return _settingsManager.AudioLatency; }
    }

    public float TimingDeviationOffset = 0.0f;
    private float? _pausedPosition;

    private DateTime? _startTime;
    private SettingsManager _settingsManager;

    public float TotalOffsetAdjust
    {
        get { return UserAudioLatency + TimingDeviationOffset + EngineOffset; }
    }

    [SerializeField] private float _audioTimingDeviation;
    public float GetSongPosition(bool ignorePause = false)
    {
        if (_startTime == null || CurrentSong == null)
        {
            return 0.0f;
        }

        if (!ignorePause && _pausedPosition.HasValue)
        {
            return _pausedPosition.Value;
        }

        var audioTime = (float)(DateTime.Now - _startTime.Value).TotalSeconds;
        audioTime += CurrentSong.AudioStart;
        return audioTime - CurrentSong.Offset + TotalOffsetAdjust;
        // return _audioSource.time - CurrentSong.Offset + EngineOffset + UserAudioLatency;
    }

    /// <summary>
    /// Returns the current position of the audio, in seconds. This is determined from the AudioSource, rather than the SongManager's internal timekeeping, and ignores the song's Offset and AudioStart.
    /// This also ignores EngineOffset, UserAudioLatency and TimingDeviationOffset
    /// </summary>
    /// <returns>The current audio position, in seconds.</returns>
    public float GetRawAudioPosition()
    {
        return _audioSource.time;
    }

    /// <summary>
    /// Sets the audio position of the AudioSource, in seconds.
    /// </summary>
    /// <param name="position">The new audio position, in seconds.</param>
    public void SetAudioPosition(float position)
    {
        var length = _audioSource.clip.length;
        _audioSource.time = Mathf.Clamp(position, 0.0f, length);
        ForceAudioResync();
    }

    public float GetAudioLength()
    {
        return _audioSource.clip.length;
    }

    /// <summary>
    /// Returns the current song position in beats, with 0 being the first playable beat (after offset).
    /// </summary>
    /// <returns>The current song position in beats.</returns>
    public float GetSongPositionInBeats()
    {
        var msTime = GetSongPosition();
        return msTime * CurrentSong.Bpm / 60;
    }
    public float GetSongEndInBeats()
    {
        return CurrentSong.LengthInBeats;
    }

    public float GetPlayableLength()
    {
        return CurrentSong.Length - CurrentSong.Offset;
    }

    public int GetCurrentSectionIndex()
    {
        var position = GetSongPositionInBeats();
        if (CurrentSong == null)
        {
            return -1;
        }

        return CurrentSong.GetSectionIndex(position);
    }

    public string GetCurrentSectionName()
    {
        var position = GetSongPositionInBeats();
        if (CurrentSong == null)
        {
            return "";
        }

        return CurrentSong.GetSectionName(position);
    }

    public void ForceAudioResync()
    {
        GetAudioTimingDeviation();
        TimingDeviationOffset = _audioTimingDeviation;
    }

    private void GetAudioTimingDeviation()
    {
        if (!IsSongPlaying || _startTime == null || CurrentSong == null)
        {
            _audioTimingDeviation = 0.0f;
            return;
        }

        var audioTime = (float)(DateTime.Now - _startTime.Value).TotalSeconds;
        audioTime += CurrentSong.AudioStart;
        _audioTimingDeviation = (_audioSource.time - audioTime);
    }

    public void LoadSong(SongData song, Action onCompleted, Action onFailed = null)
    {
        StopSong();
        CurrentSong = song;
        _startTime = null;
        Debug.Assert(CurrentSong.Bpm != 0.0f, "Song Bpm is 0!");

        Debug.Log($"Loading audio file located at {song.AudioPath}");
        var url = Helpers.PathToUri(song.AudioPath);
        StartCoroutine(LoadSongCoroutine(url, onCompleted, onFailed));
    }

    IEnumerator LoadSongCoroutine(string url, Action onCompleted, Action onFailed = null)
    {

        using (var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
        {
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.LogError("Error occurred while loading audio: " + request.error);
                onFailed?.Invoke();
                yield break;
            }

            var clip = DownloadHandlerAudioClip.GetContent(request);
            _audioSource.clip = clip;
            Debug.Log("Song Load complete. ");
            onCompleted?.Invoke();
        }

    }

    public void StartSong()
    {

        if (CurrentSong == null || _audioSource.clip == null)
        {
            throw new InvalidOperationException("StartSong() called but audio has not been loaded yet.");
        }

        PlayFromPosition(CurrentSong.AudioStart);
    }

    public void PlayFromPosition(float position)
    {
        if (CurrentSong == null || _audioSource.clip == null)
        {
            throw new InvalidOperationException("PlayFromPosition() called but audio has not been loaded yet.");
        }

        _pausedPosition = null;
        _audioSource.time = position;
        _audioSource.Play();
        _startTime = DateTime.Now;
        ForceAudioResync();
    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _settingsManager = FindObjectOfType<SettingsManager>();
    }

    void FixedUpdate()
    {
        if (!IsSongPlaying)
        {
            return;
        }

        GetAudioTimingDeviation();

        if (Mathf.Abs(_audioTimingDeviation) > 0.01f)
        {
            //     TimingDeviationOffset -= (_audioTimingDeviation / 2);
        }
    }

    public void PauseSong(bool isPaused)
    {
        if (isPaused)
        {
            _pausedPosition = this.GetSongPosition(true);
            _audioSource.Pause();
        }
        else
        {
            _pausedPosition = null;
            _audioSource.Play();
            ForceAudioResync();
        }
    }

    public void StopSong()
    {
        _audioSource.Stop();
    }

    public void StartSongFade()
    {
        GameplayMusicFader.StartFadeOut();
    }

    public void StopSongFade()
    {
        GameplayMusicFader.StopFadeOut();
    }
}
