using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : ScreenManager
{
    public Text TxtHeading;
    public GameObject SecretCodeEntryPrefab;
    public GameObject SecretCodeListContainer;
    public override void OnPlayerInput(InputEvent inputEvent)
    {
        if (inputEvent.Action == InputAction.A || inputEvent.Action == InputAction.B || inputEvent.Action == InputAction.Pause || inputEvent.Action == InputAction.Back)
        {
            UnlockEndingScene();
             var args = CoreManager.SceneLoadArgs;

            var destination = (args != null && args.TryGetValue("DestinationScene", out var temp) && temp is GameScene scene)
                ? scene
                : GameScene.MainMenu;
            base.SceneTransition(destination);
        }
    }

    private void UnlockEndingScene()
    {
        CoreManager.Settings.ShowEndingInOptions = true;
        CoreManager.Settings.Save();
    }

    void Awake()
    {
        FindCoreManager();
    }

    void Start()
    {
        SetPlayerName();
        ShowSecretCodes();
    }

    private void SetPlayerName()
    {
        var playerNames = GetPlayerNamesToCongratulate();


        TxtHeading.text = $"Congratulations {playerNames}!";
    }

    private string GetPlayerNamesToCongratulate()
    {
        string playerNames = "";
        var args = CoreManager.SceneLoadArgs;
        if (args == null)
        {
            return "";
        }

        args.TryGetValue("PlayerNamesToCongratulate", out var temp);

        if (temp != null)
        {
            playerNames = temp.ToString();
        }
        return playerNames;
    }

    private SecretCode[] _codesToSkip = { SecretCode.AbsolutelyNothing, SecretCode.EnableExtraDifficulty };

    private void ShowSecretCodes()
    {
        SecretCodeListContainer.ClearChildren();
        foreach (var code in SecretCodeHandler.SecretCodes)
        {
            if (_codesToSkip.Contains(code.Key))
            {
                continue;
            }

            var gameObj = Instantiate(SecretCodeEntryPrefab, SecretCodeListContainer.transform);
            var entry = gameObj.GetComponent<SecretCodeListEntry>();
            entry.DisplayCode(code.Key.ToString(), code.Value);
            gameObj.name = $"SecretCode: {code.Key}";
        }
    }
}
