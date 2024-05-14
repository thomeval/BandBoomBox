using System.Collections;
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

    public InputDevice CurrentInputDevice
    {
        get
        {
            if (!_playerInput.devices.Any())
            {
                return null;
            }

            return _playerInput.devices[0];
        }
    }
    public bool ModifierShift;
    public bool ModifierCtrl;
    public bool ModifierAlt;

    public float RumblePower = 0.4f;
    public float RumbleDuration = 0.05f;

    private GameObject _coreManager;

    private Player _player;
    // Start is called before the first frame update

    private readonly Dictionary<string, string> _devices = new()
    {
        {"Keyboard", "WASD"},
        {"XInputController", "ABXY"},
        {"SwitchProControllerHID", "BAYX" },
        {"PS4", "Symbols" },
        {"Gamepad", "ABXY"}
    };

    private const string DEFAULT_DEVICE = "None";

    private PlayerInput _playerInput;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _coreManager = GameObject.Find("CoreManager");
        _player = this.GetComponentInParent<Player>();
    }

    void Start()
    {
        OnControlsChanged(_playerInput);
    }

    #region Gameplay Input

    void OnA(InputValue value)
    {
        RegisterInput(value, InputAction.A);
    }
    void OnB(InputValue value)
    {
        RegisterInput(value, InputAction.B);
    }
    void OnX(InputValue value)
    {
        RegisterInput(value, InputAction.X);
    }
    void OnY(InputValue value)
    {
        RegisterInput(value, InputAction.Y);
    }

    void OnLeft(InputValue value)
    {
        RegisterInput(value, InputAction.Left);
    }
    void OnUp(InputValue value)
    {
        RegisterInput(value, InputAction.Up);
    }
    void OnRight(InputValue value)
    {
        RegisterInput(value, InputAction.Right);
    }
    void OnDown(InputValue value)
    {
        RegisterInput(value, InputAction.Down);
    }

    void OnLB(InputValue value)
    {
        RegisterInput(value, InputAction.LB);
    }
    void OnLT(InputValue value)
    {
        RegisterInput(value, InputAction.LT);
    }
    void OnRB(InputValue value)
    {
        RegisterInput(value, InputAction.RB);
    }
    void OnRT(InputValue value)
    {
        RegisterInput(value, InputAction.RT);
    }
    void OnPause(InputValue value)
    {
        RegisterInput(value, InputAction.Pause);
    }

    void OnTurbo(InputValue value)
    {
        RegisterInput(value, InputAction.Turbo);
    }
    #endregion

    #region Editor Inputs

    void OnEditor_NoteA(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteA, InputAction.Editor_NoteReleaseAnyB, InputAction.Editor_NoteAnyB);
    }
    void OnEditor_NoteB(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteB, InputAction.Editor_NoteReleaseAnyB, InputAction.Editor_NoteAnyB);
    }
    void OnEditor_NoteX(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteX, InputAction.Editor_NoteReleaseAnyB, InputAction.Editor_NoteAnyB);
    }
    void OnEditor_NoteY(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteY, InputAction.Editor_NoteReleaseAnyB, InputAction.Editor_NoteAnyB);
    }

    void OnEditor_NoteDown(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteDown, InputAction.Editor_NoteReleaseAnyD, InputAction.Editor_NoteAnyD);
    }
    void OnEditor_NoteRight(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteRight, InputAction.Editor_NoteReleaseAnyD, InputAction.Editor_NoteAnyD);
    }
    void OnEditor_NoteLeft(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteLeft, InputAction.Editor_NoteReleaseAnyD, InputAction.Editor_NoteAnyD);
    }
    void OnEditor_NoteUp(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteUp, InputAction.Editor_NoteReleaseAnyD, InputAction.Editor_NoteAnyD);
    }

    void OnEditor_NoteLB(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteLB, InputAction.Editor_NoteReleaseAnyT, InputAction.Editor_NoteLT);
    }
    void OnEditor_NoteRB(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteRB, InputAction.Editor_NoteReleaseAnyT, InputAction.Editor_NoteRT);
    }
    void OnEditor_NoteLT(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteLT, InputAction.Editor_NoteReleaseAnyT, InputAction.Editor_NoteAnyT);
    }
    void OnEditor_NoteRT(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_NoteRT, InputAction.Editor_NoteReleaseAnyT, InputAction.Editor_NoteAnyT);
    }

    void OnEditor_Confirm(InputValue value)
    {
        RegisterInput(value, InputAction.Editor_Confirm);
    }
    void OnEditor_PlayPause(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_PlayPause, InputAction.Editor_PlayFromBeginning);
    }

    void OnEditor_StepLeft(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_StepLeft, InputAction.Editor_MeasureLeft, InputAction.Editor_SectionLeft);
    }
    void OnEditor_StepRight(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_StepRight, InputAction.Editor_MeasureRight, InputAction.Editor_SectionRight);
    }

    void OnEditor_StepSizeUp(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_StepSizeUp, InputAction.Editor_ZoomIn);
    }

    void OnEditor_StepSizeDown(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_StepSizeDown, InputAction.Editor_ZoomOut);
    }

    void OnEditor_JumpToStart(InputValue value)
    {
        RegisterInput(value, InputAction.Editor_JumpToStart);
    }
    void OnEditor_JumpToEnd(InputValue value)
    {
        RegisterInput(value, InputAction.Editor_JumpToEnd);
    }

    void OnEditor_DeleteNote(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_DeleteNote, InputAction.Editor_ClearRegion);
    }
    void OnEditor_SelectRegion(InputValue value)
    {
        ChooseAndRegisterInput(value, InputAction.Editor_SelectRegion, InputAction.Editor_SwapNoteHands);
    }

    void OnEditor_Copy(InputValue value)
    {
        ChooseAndRegisterInput(value, null, null, InputAction.Editor_Copy);
    }
    void OnEditor_Cut(InputValue value)
    {
        ChooseAndRegisterInput(value, null, null, InputAction.Editor_Cut);
    }
    void OnEditor_Paste(InputValue value)
    {
        ChooseAndRegisterInput(value, null, null, InputAction.Editor_Paste, null, InputAction.Editor_PasteInverted);
    }

    void OnEditor_ModifierShift(InputValue value)
    {
        ModifierShift = value.isPressed;
    }

    void OnEditor_ModifierCtrl(InputValue value)
    {
        ModifierCtrl = value.isPressed;
    }

    void OnEditor_ModifierAlt(InputValue value)
    {
        ModifierAlt = value.isPressed;
    }
    #endregion

    #region Input Processing

    private void ChooseAndRegisterInput(InputValue value, InputAction? unmodifiedAction, InputAction? shiftAction = null, InputAction? ctrlAction = null, InputAction? altAction = null, InputAction? ctrlShiftAction = null)
    {
        var resultAction = unmodifiedAction;

        if (ModifierCtrl && ModifierShift)
        {
            resultAction = ctrlShiftAction;
        }
        else if (ModifierShift)
        {
            resultAction = shiftAction;
        }
        else if (ModifierCtrl)
        {
            resultAction = ctrlAction;
        }
        else if (ModifierAlt)
        {
            resultAction = altAction;
        }

        if (resultAction == null)
        {
            return;
        }

        RegisterInput(value, resultAction.Value);
    }

    void RegisterInput(InputValue value, InputAction action)
    {
        var inputEvent = new InputEvent { Player = _player.Slot, Action = action, IsPressed = value.isPressed };
        _coreManager.SendMessage("OnPlayerInput", inputEvent);
    }

    #endregion

    void OnBack(InputValue value)
    {
        RegisterInput(value, InputAction.Back);
    }

    void OnControlsChanged(PlayerInput input)
    {
        if (_player == null)
        {
            return;
        }
        var preferredController = GetPreferredControllerType();

        // TODO: This event fires if the controller used by the player changes OR when any of the InputBindings change. Keep track of the previous device, and only fire the event if it changes.
        // _coreManager.SendMessage("OnPlayerControlsChanged",
        //        new ControlsChangedArgs { Player = _player.Slot, ControllerType = preferredController, Device = _playerInput.devices[0]?.name });
    }

    void OnDeviceLost(PlayerInput input)
    {
        _coreManager.SendMessage("OnDeviceLost", new DeviceLostArgs { Player = _player.Slot });
    }

    public string ControllerType
    {
        get
        {

            return _playerInput.devices.FirstOrDefault()?.name ?? "None";
        }
    }

    public string GetPreferredControllerType()
    {
        _playerInput ??= GetComponent<PlayerInput>();
        if (ControllerType == "None") // No controller connected
        {
            Debug.LogWarning("Controller Disconnected for Player: " + _player.Slot);
            return null;
        }


        var match = _devices.Keys.FirstOrDefault(e => this.ControllerType.StartsWith(e));

        string result;
        if (match != null)
        {
            result = _devices[match];
        }
        else
        {
            Debug.LogWarning($"Unknown Device on Player {_player.Slot}: {this.ControllerType}");
            result = DEFAULT_DEVICE;
        }

        return result;
    }

    public void TriggerRumble(float power, float duration)
    {
        StartCoroutine(DoRumble(power, duration));
    }

    private IEnumerator DoRumble(float power, float duration)
    {
        var gamepad = CurrentInputDevice as Gamepad;

        if (gamepad == null)
        {
            yield break;
        }

        gamepad.SetMotorSpeeds(power, power);
        yield return new WaitForSeconds(duration);
        gamepad.ResetHaptics();

    }
}
