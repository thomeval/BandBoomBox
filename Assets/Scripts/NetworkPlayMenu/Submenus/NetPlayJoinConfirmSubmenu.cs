using UnityEngine.UI;

public class NetPlayJoinConfirmSubmenu : NetPlaySubmenuBase
{
    public override NetPlayMenuState NetPlayMenuState => NetPlayMenuState.JoinConfirmMenu;

    public bool IsBusy;

    public Text TxtJoinProgressMessage;

    public string JoinProgressMessage
    {
        get => TxtJoinProgressMessage.text;
        set => TxtJoinProgressMessage.text = value;
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        if (!inputEvent.IsPressed)
        {
            return;
        }
        switch (inputEvent.Action)
        {
            case InputAction.Back:
            case InputAction.B:
                ParentManager.PlaySfx(SoundEvent.SelectionCancelled);
                ParentManager.NetPlayMenuState = NetPlayMenuState.JoinMenu;
                IsBusy = false;
                break;
            case InputAction.A:
            case InputAction.Pause:

                if (!IsBusy)
                {
                    IsBusy = true;
                    ParentManager.Connect();
                    ParentManager.PlaySfx(SoundEvent.SelectionConfirmed);
                }

                break;
        }
    }

    public override void UpdateDisplayedValues()
    {
        JoinProgressMessage = "";
    }
}