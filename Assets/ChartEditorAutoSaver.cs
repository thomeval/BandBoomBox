using System;
using UnityEngine;

public class ChartEditorAutoSaver : MonoBehaviour
{
    private ChartEditorManager _chartEditorManager;

    [SerializeField]
    private int _autoSaveIntervalMinutes = 0;
    public int AutoSaveIntervalMinutes
    {
        get { return _autoSaveIntervalMinutes; }
        set
        {
            _autoSaveIntervalMinutes = value;
            this.enabled = value > 0;
            ResetTimer();
        }
    }

    [SerializeField]
    [TextArea]
    private DateTime _nextAutoSaveTime = DateTime.Now;

    private void Awake()
    {
        Helpers.AutoAssign(ref _chartEditorManager);
    }

    private void FixedUpdate()
    {
        if (AutoSaveIntervalMinutes <= 0)
        {
            this.enabled = false;
            return;
        }

        if (DateTime.Now >= _nextAutoSaveTime)
        {
            _chartEditorManager.SaveChart();
            ResetTimer();
        }
    }

    public void ResetTimer()
    {
        _nextAutoSaveTime = DateTime.Now.AddMinutes(_autoSaveIntervalMinutes);

        Debug.Log("Auto-save timer reset. Next save at: " + (AutoSaveIntervalMinutes <= 0 ? "Never" : _nextAutoSaveTime));
    }
}
