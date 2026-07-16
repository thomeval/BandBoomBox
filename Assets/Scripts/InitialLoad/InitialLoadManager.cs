using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class InitialLoadManager : ScreenManager
{
    public Text TxtLoading;
    public Text TxtSongCount;
    public bool LoadingComplete;

    void Awake()
    {
        FindCoreManager();
    }
    void Start()
    {
        TxtLoading.text = "Loading. Please wait...";
        Debug.Log($"App Save Folder: {Helpers.AppSaveFolder}");
        CoreManager.LoadSettings();
        CoreManager.SongLibrary.LoadSongsCompleted += SongLibrary_LoadSongsCompleted;
        CoreManager.SongLibrary.LoadSongs();
        CoreManager.HighScoreManager.Load();
        CoreManager.ProfileManager.ProfilesPath = CoreManager.Settings.ProfilesPath;
        CoreManager.ProfileManager.Load();
        CoreManager.ProfileManager.InitPlayerHighScoreCache(CoreManager.SongLibrary.Songs);
        CoreManager.Settings.ShowEndingInOptions = CoreManager.ProfileManager.AnyPlayerSeenEnding();
    }

    private void SongLibrary_LoadSongsCompleted(object sender, System.EventArgs e)
    {
        LoadingComplete = true;
        TxtLoading.text = "Loading complete.";
        SceneTransition(GameScene.MainMenu);
    }

    void Update()
    {
        TxtSongCount.text = CoreManager.SongLibrary.Songs.Count + " Songs Loaded";
    }

}
