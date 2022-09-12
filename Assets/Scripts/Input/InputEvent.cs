using System.Collections.Generic;

public class InputEvent
{
    public int Player { get; set; }
    public InputAction Action { get; set; }
    public bool IsPressed { get; set; }

    private static readonly Dictionary<InputAction, InputAction> _editorToGameplayActions = new()
    {
        { InputAction.Editor_StepLeft, InputAction.Left },
        { InputAction.Editor_StepRight, InputAction.Right },
        { InputAction.Editor_StepSizeUp, InputAction.Up },
        { InputAction.Editor_StepSizeDown, InputAction.Down },
        { InputAction.Left, InputAction.Left },
        { InputAction.Right, InputAction.Right },
        { InputAction.Up, InputAction.Up },
        { InputAction.Down, InputAction.Down },
        { InputAction.Editor_PlayPause, InputAction.A},
        { InputAction.Editor_Confirm, InputAction.A},
        { InputAction.Back, InputAction.Back}
    };

    /// <summary>
    /// Converts the provided InputEvent into a gameplay InputEvent by translating the action into the corresponding action from the Gameplay action map.
    /// If the provided InputEvent's action doesn't relate to a gameplay Input Action, null is returned.
    /// If the provided InputEvent's action is already from the gameplay action map, a copy of the provided InputEvent object is returned.
    /// </summary>
    /// <param name="inputEvent">The InputEvent to translate.</param>
    /// <returns>A new InputEvent with the same Player and IsPressed value, with an Action value translated into an action from the Gameplay action map. Returns null if no corresponding input action exists.</returns>
    public static InputEvent AsGameplayEvent(InputEvent inputEvent)
    {

        InputAction? action = null;

        if (_editorToGameplayActions.ContainsKey(inputEvent.Action))
        {
            action = _editorToGameplayActions[inputEvent.Action];
        }

        if (action == null)
        {
            return null;
        }

        return new InputEvent { Player = inputEvent.Player, IsPressed = inputEvent.IsPressed, Action = action.Value };

    }

}

