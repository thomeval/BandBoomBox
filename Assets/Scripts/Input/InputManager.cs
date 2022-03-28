using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public bool ControllerConnected
    {
        get { return !_playerInput.hasMissingRequiredDevices; }
    }

    private GameObject _coreManager;

    private Player _player;
    // Start is called before the first frame update

    private readonly Dictionary<string, string> _devices = new Dictionary<string, string>()
    {
        {"Keyboard", "WASD"},
        {"XInputController", "ABXY"},
        {"SwitchProControllerHID", "BAYX" }      
    };

    private const string DEFAULT_DEVICE = "Generic";

    private PlayerInput _playerInput;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _coreManager = GameObject.Find("CoreManager");
        _player = this.GetComponentInParent<Player>();
        var _manager = this.GetComponent<PlayerInputManager>();
    }

    void Start()
    {
        OnControlsChanged(_playerInput);
    }


    #region Gameplay Input

    void RegisterInput(InputValue value, string action)
    {
        var inputEvent = new InputEvent { Player = _player.Slot, Action = action, IsPressed = value.isPressed };
        _coreManager.SendMessage("OnPlayerInput", inputEvent);
    }
    void OnA(InputValue value)
    {
        RegisterInput(value, "A");
    }
    void OnB(InputValue value)
    {
        RegisterInput(value, "B");
    }
    void OnX(InputValue value)
    {
        RegisterInput(value, "X");
    }
    void OnY(InputValue value)
    {
        RegisterInput(value, "Y");
    }

    void OnLeft(InputValue value)
    {
        RegisterInput(value, "Left");
    }
    void OnUp(InputValue value)
    {
        RegisterInput(value, "Up");
    }
    void OnRight(InputValue value)
    {
        RegisterInput(value, "Right");
    }
    void OnDown(InputValue value)
    {
        RegisterInput(value, "Down");
    }

    void OnLB(InputValue value)
    {
        RegisterInput(value, "LB");
    }
    void OnLT(InputValue value)
    {
        RegisterInput(value, "LT");
    }
    void OnRB(InputValue value)
    {
        RegisterInput(value, "RB");
    }
    void OnRT(InputValue value)
    {
        RegisterInput(value, "RT");
    }
    void OnPause(InputValue value)
    {
        RegisterInput(value, "Pause");
    }

    void OnTurbo(InputValue value)
    {
        RegisterInput(value, "Turbo");
    }
    #endregion

    void OnBack(InputValue value)
    {
        RegisterInput(value, "Back");
    }

    void OnControlsChanged(PlayerInput input)
    {
        if (_player == null)
        {
            return;
        }
        var preferredController = GetPreferredControllerType();
        _coreManager.SendMessage("OnPlayerControlsChanged",
                new ControlsChangedArgs { Player = _player.Slot, ControllerType = preferredController });
    }

    void OnDeviceLost(PlayerInput input)
    {
        _coreManager.SendMessage("OnDeviceLost", new DeviceLostArgs { Player = _player.Slot });
    }

    public string GetPreferredControllerType()
    {
        _playerInput ??= GetComponent<PlayerInput>();
        if (!_playerInput.devices.Any())
        {
            Debug.LogWarning("Controller Disconnected for Player: " + _player.Slot);
            return null;
        }

        var newDeviceName = _playerInput.devices[0].name;
        var match = _devices.Keys.FirstOrDefault(e => newDeviceName.StartsWith(e));

        string result;
        if (match != null)
        {
            result = _devices[match];
        }
        else
        {
            Debug.LogWarning($"Unknown Device on Player {_player.Slot}: {newDeviceName}");
            result = DEFAULT_DEVICE;
        }

        return result;
    }

    public void SetActionMap(ActionMapType actionMap)
    {
        _playerInput.SwitchCurrentActionMap(actionMap.ToString());
    }
}
