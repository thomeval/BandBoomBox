using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public string[] SongFolders =
    {
        Path.Combine("%StreamingAssetsFolder%","Songs"),
        Path.Combine("%AppSaveFolder%","Songs")
    };

    public string LastPlayedSong;
    public string LastSortSongMode = "TITLE";

    [NonSerialized]
    public string SettingsFilePath;

    [Header("Gameplay Settings")]
    public bool EnableNerfDifficulty = false;
    public bool EnableExtraDifficulty = false;
    public bool EnableMomentumOption = false;

    [Header("Audio Settings")]
    public float MasterVolume = 0.5f;
    public float GameplaySfxVolume = 1.0f;
    public float GameplayMusicVolume = 0.8f;
    public float MistakeVolume = 1.0f;
    public float MenuSfxVolume = 1.0f;
    public float MenuMusicVolume = 0.8f;
    public float AudioLatency;

    [Header("Graphics Settings")]
    public int TargetFrameRate = -1;
    public string ScreenResolution;
    public bool VSyncEnabled = false;
    public FullScreenMode FullScreenMode = FullScreenMode.FullScreenWindow;

    [Header("Editor Settings")]
    public string LastUsedEditorPath;
    public string DefaultChartAuthor;
    public string EditorLastUsedNoteLabels = "WASD";
    public int EditorAutoSaveIntervalMinutes = 10;
    public bool EditorAutoStepForward = false;
    public bool EditorAllowAllNotes = false;
    public int EditorScrollSpeed = 500;

    // Start is called before the first frame update
    void Awake()
    {
        if (string.IsNullOrEmpty(SettingsFilePath))
        {
            SettingsFilePath = Path.Combine(Helpers.AppSaveFolder, "Settings.json");
        }
    }

    public string[] GetResolvedSongFolders()
    {
        return SongFolders.Select(Helpers.ResolvePath).ToArray();
    }
    public void Save()
    {

        var targetFolder = Path.GetDirectoryName(Helpers.ResolvePath(SettingsFilePath));

        if (string.IsNullOrEmpty(targetFolder))
        {
            throw new ArgumentNullException(targetFolder);
        }

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var json = JsonUtility.ToJson(this, true);
        File.WriteAllText(SettingsFilePath, json);
        Debug.Log($"Settings saved to {SettingsFilePath} successfully.");
    }

    public void Load()
    {
        if (!File.Exists(SettingsFilePath))
        {
            Debug.Log($"Settings file was not found: {SettingsFilePath}");

            Save();
            return;
        }

        var json = File.ReadAllText(SettingsFilePath);
        JsonUtility.FromJsonOverwrite(json, this);

        if (ScreenResolution.IndexOf("x") == -1)
        {
            ScreenResolution = string.Format("{0}x{1}", Display.main.systemWidth, Display.main.systemHeight);
        }

        Debug.Log("Settings loaded successfully.");
    }

    public bool IsDifficultyVisible(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Extra:
                return this.EnableExtraDifficulty;
            case Difficulty.Nerf:
                return this.EnableNerfDifficulty;
            default:
                return true;
        }
    }
}
