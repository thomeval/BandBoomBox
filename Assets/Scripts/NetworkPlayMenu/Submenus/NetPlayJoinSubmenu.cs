using UnityEngine.UI;

public class NetPlayJoinSubmenu : NetPlaySubmenuBase
{
    public override NetPlayMenuState NetPlayMenuState => NetPlayMenuState.JoinMenu;

    public InputField TxtJoinIpAddress;
    public InputField TxtJoinPort;
    public InputField TxtJoinPassword;
    public Text LblJoinMessage;

    public ushort JoinPort
    {
        get => ushort.Parse(TxtJoinPort.text);
        set => TxtJoinPort.text = value.ToString();
    }
    public string JoinIpAddress
    {
        get => TxtJoinIpAddress.text;
        set => TxtJoinIpAddress.text = value;
    }
    public string JoinPassword
    {
        get => TxtJoinPassword.text;
        set => TxtJoinPassword.text = value;
    }

    public string Message
    {
        get => LblJoinMessage.text;
        set => LblJoinMessage.text = value;
    }

    public override void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "IP Address":
                TxtJoinIpAddress.Select();
                break;
            case "Port":
                TxtJoinPort.Select();
                break;
            case "Password":
                TxtJoinPassword.Select();
                break;
            case "Connect":
                var hash = ComputeHash(JoinPassword);
                ParentManager.Connect(hash);
                break;
            case "Back":
                ParentManager.NetPlayMenuState = NetPlayMenuState.MainMenu;
                break;
        }
    }

    public override bool MenuInputActive()
    {
        return !(TxtJoinIpAddress.isFocused || TxtJoinPassword.isFocused || TxtJoinPort.isFocused);
    }

    public override void LoadFromSettings(SettingsManager settingsManager)
    {
        JoinPort = settingsManager.NetGameJoinPort;
        JoinIpAddress = settingsManager.NetGameJoinIpAddress;
    }

    public override void SaveToSettings(SettingsManager settingsManager)
    {
        settingsManager.NetGameJoinPort = JoinPort;
        settingsManager.NetGameJoinIpAddress = JoinIpAddress;
    }

}
