using System.Collections.Generic;
using UnityEngine;

public class SecretCodeHandler : MonoBehaviour
{
    private CoreManager _coreManager;
    private readonly Dictionary<SecretCode, InputAction[]> _inputCodes = new()
        {
            { SecretCode.EnableNerfDifficulty, new [] { InputAction.Left, InputAction.Left, InputAction.Left, InputAction.Right, InputAction.Right, InputAction.Right, InputAction.Left, InputAction.Right, InputAction.Pause } },
            { SecretCode.EnableMomentumOption, new [] { InputAction.B, InputAction.X, InputAction.Down, InputAction.A, InputAction.B, InputAction.X, InputAction.Down, InputAction.A, InputAction.Pause  } },
            { SecretCode.EnableExtraDifficulty, new [] { InputAction.Down, InputAction.Down, InputAction.Down } }
        };

    private readonly Dictionary<SecretCode, int> _inputCodeProgress = new();

    private Dictionary<SecretCode, string> _activationMessages = new()
    {
        { SecretCode.EnableExtraDifficulty, "Extra difficulty unlocked!" },
        { SecretCode.EnableNerfDifficulty, "N.E.R.F. difficulty unlocked!" },
        { SecretCode.EnableMomentumOption, "Momentum scroll speed option unlocked!" },
    };

    public SecretCode? ActivatedCode { get; private set; }
    public string Message
    {
        get
        {
            if (ActivatedCode == null)
            {
                return "";
            }

            return _activationMessages[ActivatedCode.Value];
        }
    }

    private void Awake()
    {
        Helpers.AutoAssign(ref _coreManager);

        foreach (var code in _inputCodes.Keys)
        {
            _inputCodeProgress.Add(code, 0);
        }
    }

    public void HandleInput(InputAction action)
    {
        foreach (var currentCode in _inputCodes.Keys)
        {
            var progress = _inputCodeProgress[currentCode];
            var next = _inputCodes[currentCode][progress];

            if (action == next)
            {
                _inputCodeProgress[currentCode]++;
            }
            else
            {
                _inputCodeProgress[currentCode] = 0;
            }

            if (_inputCodeProgress[currentCode] == _inputCodes[currentCode].Length)
            {
                Debug.Log("Secret code activated: " + currentCode);
                ActivatedCode = currentCode;

                _inputCodeProgress[currentCode] = 0;
            }
        }
    }

    public void RunActivatedCode()
    {
        if (ActivatedCode == null)
        {
            return;
        }

        switch (ActivatedCode.Value)
        {
            // Extra difficulty should be enabled by default, but this is a failsafe to allow it to be enabled without manually editing the settings file.
            case SecretCode.EnableExtraDifficulty:
                _coreManager.Settings.EnableExtraDifficulty = true;
                _coreManager.Settings.Save();
                break;
            case SecretCode.EnableNerfDifficulty:
                _coreManager.Settings.EnableNerfDifficulty = true;
                _coreManager.Settings.Save();
                break;
            case SecretCode.EnableMomentumOption:
                _coreManager.Settings.EnableMomentumOption = true;
                _coreManager.Settings.Save();
                break;
            default:
                Debug.LogWarning("Unknown secret code: " + ActivatedCode);
                break;
        }

    }
}

