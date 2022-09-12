using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


public class ControlsManager : MonoBehaviour
{
    public InputActionAsset InputActionAsset;
    public string ControlsFile = "Controls.json";

    private PlayerManager _playerManager;

    void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    public void LoadInputActions()
    {
        var path = Path.Combine(Helpers.AppSaveFolder, ControlsFile);
        if (!File.Exists(path))
        {
            return;
        }

        var json = File.ReadAllText(path);
        InputActionAsset = InputActionAsset.FromJson(json);
        Debug.Log($"Controls loaded from {path} successfully.");
        _playerManager.ApplyInputActions(json);
    }

    public void SaveInputActions()
    {
        var path = Path.Combine(Helpers.AppSaveFolder, ControlsFile);

        try
        {
            var json = InputActionAsset.ToJson();
            File.WriteAllText(path, json);
            Debug.Log($"Controls saved to {path} successfully.");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void SetActionMap(ActionMapType actionMap)
    {
        var playerInputs = FindObjectsOfType<PlayerInput>();

        foreach (var input in playerInputs)
        {
            input.SwitchCurrentActionMap(actionMap.ToString());
        }
    }
}

