using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomBindingDisplay : MonoBehaviour
{
    private ControlsManager _controlsManager;
    private OptionsManager _optionsManager;
    private RebindManager _rebindManager;

    public Text TxtMessage;
    public Color AwaitingInputColor = Color.yellow;
    public Color NormalMessageColor = Color.white;

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
        ShowMessage("Key bindings have been reset to default.");
        ApplyBindings();
    }

    private void ShowMessage(string message)
    {
        ShowMessage(message, NormalMessageColor);
    }

    private void ShowMessage(string message, Color color)
    {
        TxtMessage.text = message;
        TxtMessage.color = color;
    }

    private void ApplyBindings()
    {
        _controlsManager.ApplyCustomBindings();
        _controlsManager.SaveInputActions();
        Display(_controlsManager.CustomBindings);
    }

    public void ListenForNewBinding(string action)
    {
        ShowMessage($"Press a key to bind to '{action}'.\r\nPress ESC to cancel.", AwaitingInputColor);
        _optionsManager.PlaySfx(SoundEvent.Options_KeyBindingStart);

        _rebindManager.StartListening((newKey) =>
        {
            AddBinding(action, newKey);
        });
    }

    private void AddBinding(string action, string newKey)
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
        ShowMessage($"Bound '{action}' to '{newKey}' successfully.");
    }
}
