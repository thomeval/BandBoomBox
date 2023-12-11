using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SongPreviewManager : MonoBehaviour
{
    public AudioSource AudioSource;

    public float PreviewStart;
    public float PreviewEnd;
    public float Offset = -0.1f;

    public float FadeInTime;

    private float _previewLength;

    private float _actualPreviewStart
    {
        get { return PreviewStart + Offset; }
    }
    private float _actualPreviewEnd
    {
        get { return PreviewEnd + Offset; }
    }

    public SongData PreviewedSongData;

    private BackgroundManager _backgroundManager;

    void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        _backgroundManager = FindObjectOfType<BackgroundManager>();

    }
    // Start is called before the first frame update
    void Start()
    {
        AudioSource.volume = 0.0f;
    }

    void FixedUpdate()
    {
        if (!AudioSource.isPlaying)
        {
            return;
        }

        if (FadeInTime <= 0.0f)
        {
            AudioSource.volume = 1.0f;
            return;
        }

        var inc = Time.fixedDeltaTime / FadeInTime;
        AudioSource.volume = Math.Min(1.0f, AudioSource.volume + inc);

        var pos = AudioSource.time;

        if (pos > _actualPreviewEnd)
        {
            AudioSource.time -= _previewLength;
        }
    }

    public void PlayPreview(SongData songData)
    {
        if (!File.Exists(songData.AudioPath))
        {
            Debug.LogWarning($"Audio file for song [{songData.Title}] located at {songData.SjsonFilePath} is missing.");
            return;
        }
        var url = Helpers.PathToUri(songData.AudioPath);

        _previewLength = 60.0f / songData.Bpm * 4 * 8;          // 8 Measures
        PreviewedSongData = songData;

        StartCoroutine(LoadSongCoroutine(url, songData.Offset, songData.Offset + _previewLength));
    }

    public void StopPreviews()
    {
        AudioSource.Stop();
        AudioSource.volume = 0.0f;
    }

    /*
     * Implementation using UnityWebRequest.
     * Recommended by Unity compiler, but not used due to poor performance.
    IEnumerator LoadSongCoroutine(string url, float previewStart, float previewEnd)
    {
        var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.LogError("Error occurred while loading audio: " + request.error);
            yield break;
        }

        var clip = DownloadHandlerAudioClip.GetContent(request);
        PreviewStart = previewStart;
        PreviewEnd = previewEnd;
        StartPreview(clip);
    }
    */

    IEnumerator LoadSongCoroutine(string url, float previewStart, float previewEnd)
    {
        var www = new WWW(url);
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("Error occurred while loading audio: " + www.error);
            yield break;
        }
        var clip = www.GetAudioClip(false, true);

        PreviewStart = previewStart;
        PreviewEnd = previewEnd;
        StartPreview(clip);
    }

    private void StartPreview(AudioClip clip)
    {
        AudioSource.clip = clip;
        AudioSource.time = _actualPreviewStart;
        AudioSource.Play();

        if (clip != null)
        {
            _backgroundManager.SetBpm(PreviewedSongData.Bpm, true);
            _backgroundManager.SetSpeedMultiplier(1.0f);
        }
    }
}
