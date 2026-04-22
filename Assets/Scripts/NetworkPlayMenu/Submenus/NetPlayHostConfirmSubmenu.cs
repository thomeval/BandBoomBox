public class NetPlayHostConfirmSubmenu : NetPlaySubmenuBase
{
    public override NetPlayMenuState NetPlayMenuState => NetPlayMenuState.HostConfirmMenu;

    public bool IsBusy;

    public override void HandleInput(InputEvent inputEvent)
    {
        if (!inputEvent.IsPressed)
        {
            return;
        }
        switch (inputEvent.Action)
        {
            case InputAction.Back:
                ParentManager.PlaySfx(SoundEvent.SelectionCancelled);
                ParentManager.NetPlayMenuState = NetPlayMenuState.HostMenu;
                IsBusy = false;
                break;
            case InputAction.A:
            case InputAction.Pause:

                if (!IsBusy)
                {
                    IsBusy = true;
                    ParentManager.PlaySfx(SoundEvent.SelectionConfirmed);
                    ParentManager.StartHosting();
                }

                break;
        }
    }
}