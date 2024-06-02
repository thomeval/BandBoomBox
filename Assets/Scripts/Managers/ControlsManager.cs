using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public CustomBindingSet CustomBindings = new();
    public string CustomControlsFile = "%AppSaveFolder%/Controls.json";
    public string DefaultControlsFile = "%StreamingAssetsFolder%/DefaultControls.json";
    public const int CURRENT_BINDINGSET_VERSION = 1;

    private PlayerManager _playerManager;

    void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    public void LoadInputActions()
    {

        try
        {
            CustomBindings = LoadInputActions(CustomControlsFile);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Unable to load custom keyboard controls from {CustomControlsFile}. {e.Message} When running this game for the first time, this warning can be ignored. Attempting to load default configuration...");

            try
            {
                CustomBindings = LoadInputActions(DefaultControlsFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unable to load default keyboard controls from {DefaultControlsFile}. {ex.Message}");
            }
        }

    }

    private CustomBindingSet LoadInputActions(string path)
    {
        path = Helpers.ResolvePath(path);
        var json = File.ReadAllText(path);

        var result = JsonConvert.DeserializeObject<CustomBindingSet>(json);

        if (result.Version != CURRENT_BINDINGSET_VERSION)
        {
            throw new Exception($"Custom controls file is outdated: v{result.Version}. Expected v{CURRENT_BINDINGSET_VERSION}. ");
        }

        Debug.Log($"Controls loaded from {path} successfully. Loaded {result.Bindings.Count} bindings.");
        return result;
    }


    public void SaveInputActions()
    {
        var path = Helpers.ResolvePath(CustomControlsFile);

        try
        {
            var json = JsonConvert.SerializeObject(CustomBindings, Formatting.Indented);
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

    public void ApplyCustomBindings()
    {
        foreach (var player in _playerManager.Players)
        {
            ApplyCustomBindings(player);
        }
    }

    private void ApplyCustomBindings(Player player)
    {
        var playerInput = player.GetComponent<PlayerInput>();

        if (!this.CustomBindings.Bindings.Any())
        {
            return;
        }

        ClearKeyboardBindings(playerInput);

        foreach (var entry in this.CustomBindings.Bindings)
        {
            playerInput.actions[entry.Action].AddBinding(entry.Path);
        }

    }

    private void ClearKeyboardBindings(PlayerInput playerInput)
    {
        // playerInput.actions does not have an indexer. Convert to list first.
        var actions = playerInput.actions.ToList();

        for (var x = 0; x < actions.Count(); x++)
        {
            var action = actions[x];

            if (action.actionMap.name != "Gameplay")
            {
                continue;
            }

            for (var y = 0; y < action.bindings.Count; y++)
            {
                var binding = action.bindings[y];
                if (binding.path.StartsWith("<Keyboard>"))
                {
                    action.ChangeBinding(y).Erase();
                }
            }

        }
    }
}

