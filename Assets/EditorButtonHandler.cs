using UnityEngine;

public class EditorButtonHandler : MonoBehaviour
{
    private ChartEditorManager _parent;
    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
    }

    public void BtnPlay_Click()
    {
        SendInput(InputAction.Editor_PlayPause, ChartEditorState.Edit);
    }

    public void BtnStop_Click()
    {
        SendInput(InputAction.Back, ChartEditorState.Playback);
    }

    public void BtnPause_Click()
    {
        SendInput(InputAction.Editor_PlayPause, ChartEditorState.Playback);
    }

    public void BtnJumpToStart_Click()
    {
        SendInput(InputAction.Editor_JumpToStart, ChartEditorState.Edit);
    }

    public void BtnJumpToEnd_Click()
    {
        SendInput(InputAction.Editor_JumpToEnd, ChartEditorState.Edit);
    }

    private void SendInput(InputAction action, ChartEditorState requiredState)
    {
        if (_parent.ChartEditorState != requiredState)
        {
            return;
        }
        _parent.OnPlayerInput(new InputEvent{Action = action, IsPressed = true});
    }
}
