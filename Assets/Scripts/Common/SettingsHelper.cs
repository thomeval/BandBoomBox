using System;
using UnityEngine;

public class SettingsHelper : MonoBehaviour
{
    private CoreManager _coreManager;
    private SettingsManager _settings => _coreManager.Settings;

    private const float FLOAT_TOLERANCE = 0.0001f;

    private void Awake()
    {
        Helpers.AutoAssign(ref _coreManager);
    }

    public void ApplySettings()
    {
        _coreManager.SongLibrary.SongFolders = _settings.SongFolders;

        ApplyAudioSettings();
        ApplyGraphicsSettings();
        //  PlayerManager.ApplyInputActions();
    }

    public void ApplyAudioSettings()
    {
        ApplyAudioVolume("MasterVolume", _settings.MasterVolume);
        ApplyAudioVolume("GameplaySfxVolume", _settings.GameplaySfxVolume);
        ApplyAudioVolume("GameplayMusicVolume", _settings.GameplayMusicVolume);
        ApplyAudioVolume("MistakeSfxVolume", _settings.MistakeVolume);
        ApplyAudioVolume("MenuSfxVolume", _settings.MenuSfxVolume);
        ApplyAudioVolume("MenuMusicVolume", _settings.MenuMusicVolume);
    }

    public void ApplyAudioVolume(string mixerParam, float volume)
    {
        volume = Mathf.Max(volume, 0.00001f);
        var temp = Mathf.Log10(volume) * 20;
        _coreManager.AudioMixer.SetFloat(mixerParam, temp);
    }

    public float GetAudioVolume(string mixerParam)
    {
        _coreManager.AudioMixer.GetFloat(mixerParam, out var volume);
        return Mathf.Pow(10, volume / 20);
    }

    private void ApplyGraphicsSettings()
    {
        var resolution = _settings.ScreenResolution;
        const int DEFAULT_HEIGHT = 720;
        const int DEFAULT_WIDTH = 1280;

        if (string.IsNullOrWhiteSpace(resolution))
        {
            resolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        }

        var resParts = resolution.Trim().Split('x');
        var width = Convert.ToInt32(resParts[0]);
        var height = Convert.ToInt32(resParts[1]);

        var aspectRatio = 1.0 * width / height;
        var expectedAspectRatio = 1.0 * 16 / 9;

        if (Math.Abs(aspectRatio - expectedAspectRatio) > FLOAT_TOLERANCE)
        {
            Debug.LogWarning($"Unsupported aspect ratio detected: {width}x{height}. Using default resolution of {DEFAULT_WIDTH}x{DEFAULT_HEIGHT} instead. ");
            width = DEFAULT_WIDTH;
            height = DEFAULT_HEIGHT;
        }

        Screen.SetResolution(width, height, _settings.FullScreenMode);
        Application.targetFrameRate = _settings.TargetFrameRate;
        QualitySettings.vSyncCount = _settings.VSyncEnabled ? 1 : 0;
    }
}