using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public CoreManager CoreManager;
    public bool IgnoreReleaseInputs = false;
    public const GameScene STARTING_SCENE = GameScene.InitialLoad;
    public ActionMapType DefaultActionMapType = ActionMapType.Gameplay;

    private readonly Dictionary<string, object> _defaultSceneLoadArgs = new();
    public bool FindCoreManager()
    {
        if (CoreManager == null)
        {
            CoreManager = FindObjectOfType<CoreManager>();
        }

        if (CoreManager != null)
        {
            CoreManager.ActiveMainManager = this;
            SetActionMap(DefaultActionMapType);
            return true;
        }

        Debug.LogWarning("No CoreManager found. Transitioning to Starting Scene.");
        SceneTransition(STARTING_SCENE);
        return false;

    }

    public virtual void OnPlayerInput(InputEvent inputEvent)
    {
    }


    public virtual void OnPlayerControlsChanged(ControlsChangedArgs args)
    {

    }

    public virtual void OnPlayerJoined(Player player)
    {
    }

    public virtual void OnDeviceLost(DeviceLostArgs args)
    {
    }

    protected Player GetPlayer(int slot)
    {
        return CoreManager.PlayerManager.GetPlayer(slot);
    }

    protected void SceneTransition(GameScene gameScene, Dictionary<string, object> sceneLoadArgs = null, bool withTransition = true)
    {
        if (CoreManager != null)
        {
            CoreManager.SceneLoadArgs = sceneLoadArgs ?? _defaultSceneLoadArgs;
        }

        StartCoroutine(DoSceneTransition(gameScene, withTransition));
    }

    protected IEnumerator DoSceneTransition(GameScene gameScene, bool withTransition = true)
    
    {
        Debug.Log($"Transitioning to {gameScene}");
        var sceneName = gameScene + "Scene";

        if (CoreManager == null || !withTransition)
        {
            SceneManager.LoadScene(sceneName);
         
            yield break;    // Return statement, but for a coroutine.
        }

        yield return  CoreManager.SceneTransitionManager.RunTransitionStart();
        CoreManager?.MenuMusicManager?.PlaySceneMusic(gameScene);
        SceneManager.LoadScene(sceneName);
        yield return CoreManager.SceneTransitionManager.RunTransitionEnd();

    }

    public void PlaySfx(SoundEvent soundEvent)
    {
        CoreManager.SoundEventHandler.PlaySfx(soundEvent);
    }

    public virtual void SetActionMap(ActionMapType actionMapType)
    {
        CoreManager.ControlsManager.SetActionMap(actionMapType);
    }

}

