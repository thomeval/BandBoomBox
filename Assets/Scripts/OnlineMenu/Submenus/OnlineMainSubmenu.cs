using System.Linq;
using System.Net;
using UnityEngine.UI;

public class OnlineMainSubmenu : OnlineSubmenuBase
{
    public override OnlineMenuState OnlineMenuState => OnlineMenuState.MainMenu;
    public Text TxtLocalIps;

    public override void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Host Game":
                ParentManager.OnlineMenuState = OnlineMenuState.HostMenu;
                break;
            case "Join Game":
                ParentManager.OnlineMenuState = OnlineMenuState.JoinMenu;
                break;
            case "Back To Main Menu":
                ParentManager.ReturnToMainMenu();
                break;
        }
    }

    public void GetLocalIps()
    {
        TxtLocalIps.text = "Your IP's: ";

        try
        {
            var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                            .Where(e => e.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            foreach (var address in addresses)
            {
                TxtLocalIps.text += address.ToString() + ", ";
            }
        }
        catch (System.Exception e)
        {
            TxtLocalIps.text += "[Unknown]";
        }
    }

}
