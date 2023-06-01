using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorManager : ScreenManager
{
    public GameObject PageContainer;

    private readonly Dictionary<EditorPage, EditorPageManager> _pages = new();

    private EditorPage _currentPage = EditorPage.MainMenu;
    public EditorPage CurrentPage
    {
        get { return _currentPage; }
        set
        {
            _currentPage = value;
            DisplayCurrentPage();
        }
    }

    public string SongsHomePath;
    public SongData CurrentSong;

    public EventSystem EventSystem;

    public EditorPageManager CurrentPageManager
    {
        get { return _pages[CurrentPage]; }
    }



    public bool IsExistingSong;

    private void DisplayCurrentPage()
    {
        foreach (var pair in _pages)
        {
            pair.Value.gameObject.SetActive(pair.Key == CurrentPage);
        }
    }

    void Awake()
    {
        if (!FindCoreManager())
        {
            return;
        }

        var expectedHomePath = "%AppSaveFolder%/Songs";

        if (!CoreManager.SongLibrary.SongFolders.Contains(expectedHomePath))
        {
            this.SongsHomePath = Helpers.ResolvePath(CoreManager.SongLibrary.SongFolders[0]);
        }
        else
        {
            this.SongsHomePath = Helpers.ResolvePath(expectedHomePath);
        }

        PopulatePages();

        var fileSelector = (EditorFileSelectPage)_pages[EditorPage.FileSelect];
        fileSelector.DefaultPath = this.SongsHomePath;
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayCurrentPage();
    }

    private void PopulatePages()
    {
        foreach (var pageObj in PageContainer.GetChildren())
        {
            var manager = pageObj.GetComponent<EditorPageManager>();

            if (manager == null)
            {
                Debug.LogWarning($"GameObject with name {pageObj.name} is missing its EditorPageManager.");
                continue;
            }

            _pages.Add(manager.EditorPage, manager);
        }
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        if (inputEvent.IsPressed || !CurrentPageManager.IgnoreReleaseInputs)
        {
            CurrentPageManager.HandleInput(inputEvent);
        }
    }

    public void RequestMeasureTime(Action<float?> callback, float startTime = 0.0f)
    {
        var measurer = (EditorMeasureTimePage) _pages[EditorPage.MeasureTime];
        measurer.OnMeasureComplete = callback;
        CurrentPage = EditorPage.MeasureTime;
        measurer.BeginMeasure(startTime);
    }

    public void RequestMeasureBpm(Action<float?> callback, float startTime = 0.0f)
    {
        var measurer = (EditorMeasureBpmPage)_pages[EditorPage.MeasureBpm];
        measurer.OnMeasureComplete = callback;
        CurrentPage = EditorPage.MeasureBpm;
        measurer.BeginMeasure(startTime);
    }

    public void RequestFineTune(Action callback)
    {
        var tuner = (EditorFineTunePage) _pages[EditorPage.FineTune];
        tuner.OnTuneComplete = callback;
        CurrentPage = EditorPage.FineTune;
        tuner.BeginFineTune();
    }

    public void SaveCurrentSong(bool allowZeroCharts)
    {
        var validateResult = SongValidator.Validate(CurrentSong, allowZeroCharts);
        if (validateResult != "")
        {
            Debug.LogWarning("Cannot save current song due to validation failure: " + validateResult);
            return;
        }

        if (CurrentSong != null)
        {
            CoreManager.SongLibrary.SaveSongToDisk(CurrentSong);
            if (!CoreManager.SongLibrary.Contains(CurrentSong))
            {
                CoreManager.SongLibrary.Add(CurrentSong);
            }
        }
    }

    public void RequestBrowseFile(string filePattern, Action<string> callback)
    {
        var fileSelector = (EditorFileSelectPage)_pages[EditorPage.FileSelect];
        fileSelector.Show(filePattern, callback);
        CurrentPage = EditorPage.FileSelect;
    }

    public void LoadSong(string target)
    {
        try
        {
            var result = CoreManager.SongLibrary.LoadSong(target);
            CurrentSong = result;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to load song at {target} : {e}");
        }

    }

    public void RaiseSceneTransition(GameScene gameScene, Dictionary<string, object> args = null)
    {
        args ??= new();
        this.SceneTransition(gameScene, args);
    }
}
