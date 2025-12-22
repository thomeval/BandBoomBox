using System.Linq;
using System.Net;
using UnityEngine.UI;

public class NetPlayMainSubmenu : NetPlaySubmenuBase
{
    public override NetPlayMenuState NetPlayMenuState => NetPlayMenuState.MainMenu;
    public Text TxtLocalIps;

    public override void HandleInput(InputEvent inputEvent)
    {
        base.HandleInput(inputEvent);
    }

    public override void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Host Game":
                ParentManager.NetPlayMenuState = NetPlayMenuState.HostMenu;
                break;
            case "Join Game":
                ParentManager.NetPlayMenuState = NetPlayMenuState.JoinMenu;
                break;
            case "Back To Main Menu":
                ParentManager.ReturnToMainMenu();
                break;
        }
    }
}
