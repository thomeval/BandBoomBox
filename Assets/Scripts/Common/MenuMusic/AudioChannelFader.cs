using System;
using UnityEngine;

public class AudioChannelFader : MonoBehaviour
{
    public float HoldTime = 10f;
    public float FadeTime = 10f;
    public string MixerGroupName = "MenuMusic";

    [SerializeField]
    private DateTime? _startTime;
    [SerializeField]
    private float _initialVolume;
    [SerializeField]
    private bool _active = false;

    private SettingsHelper _settingsHelper;

    private void Awake()
    {
        _settingsHelper = FindObjectOfType<SettingsHelper>();
    }

    private void FixedUpdate()
    {
        if (_active)
        {
            ApplyFadeOut();
        }
    }

    private void ApplyFadeOut()
    {
        var elapsed = (DateTime.Now - _startTime.Value).TotalSeconds;

        var factor = 0.0f;

        if (elapsed > FadeTime + HoldTime)
        {
            factor = 0.0f;
            _settingsHelper.ApplyAudioVolume(MixerGroupName, factor * _initialVolume);
            _active = false;
            return;
        }
        else if (elapsed <= HoldTime)
        {
            factor = 1.0f;
        }

        factor = Mathf.InverseLerp(HoldTime, HoldTime + FadeTime, (float)elapsed);
        factor = 1.0f - factor;
        _settingsHelper.ApplyAudioVolume(MixerGroupName, factor * _initialVolume);
    }

    public void StartFadeOut()
    {
        this.gameObject.SetActive(true);
        _initialVolume = _settingsHelper.GetAudioVolume(MixerGroupName);
        _startTime = DateTime.Now;
        _active = true;
    }

    public void StopFadeOut()
    {
        _active = false;
        _settingsHelper.ApplyAudioVolume(MixerGroupName, _initialVolume);
        this.gameObject.SetActive(false);
    }
}
