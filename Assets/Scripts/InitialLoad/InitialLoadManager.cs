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

    // Start is called before the first frame update
    void Start()
    {
        TxtLoading.text = "Loading. Please wait...";
        Debug.Log($"App Save Folder: {Helpers.AppSaveFolder}");
        CoreManager.LoadSettings();
        CoreManager.SongLibrary.LoadSongsCompleted += SongLibrary_LoadSongsCompleted;
        CoreManager.SongLibrary.LoadSongs();
        CoreManager.HighScoreManager.Load();

        CoreManager.ProfileManager.Load();
    }

    private void SongLibrary_LoadSongsCompleted(object sender, System.EventArgs e)
    {
        LoadingComplete = true;
        TxtLoading.text = "Loading complete.";
        SceneTransition(GameScene.MainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        TxtSongCount.text = CoreManager.SongLibrary.Songs.Count + " Songs Loaded";
    }

}
