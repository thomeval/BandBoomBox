using System;
using UnityEngine;

public class CustomBindingDisplay : MonoBehaviour
{
    private ControlsManager _controlsManager;
    private OptionsManager _optionsManager;
    private RebindManager _rebindManager;
    void Awake()
    {
        Helpers.AutoAssign(ref _controlsManager);
        Helpers.AutoAssign(ref _optionsManager);
        Helpers.AutoAssign(ref _rebindManager);
    }

    public void Display(CustomBindingSet bindings)
    {
        foreach (var item in GetComponentsInChildren<CustomBindingDisplayItem>())
        {
            item.Display(bindings);
        }
    }

    public void ResetDefaults()
    {
        Debug.Log("Resetting to default key bindings.");
        _controlsManager.LoadDefaultInputActions();
        _optionsManager.PlaySfx(SoundEvent.Options_KeyBindingReset);
        ApplyBindings();
    }

    private void ApplyBindings()
    {
        _controlsManager.ApplyCustomBindings();
        _controlsManager.SaveInputActions();
        Display(_controlsManager.CustomBindings);
    }

    public void ListenForNewBinding(string action)
    {
        _rebindManager.StartListening((newKey) =>
        {
            if (_controlsManager == null)
            {
                throw new NullReferenceException(nameof(_controlsManager));
            }

            if (string.IsNullOrEmpty(newKey))
            {
                // Cancelled
                _optionsManager.PlaySfx(SoundEvent.Options_KeyBindingCancelled);
                return;
            }

            Debug.Log($"New binding requested: {action} -> {newKey}");
            _controlsManager.CustomBindings.BindKey(action, newKey);
            ApplyBindings();
            _optionsManager.PlaySfx(SoundEvent.Options_KeyBindingEnd);
        });
    }
}
