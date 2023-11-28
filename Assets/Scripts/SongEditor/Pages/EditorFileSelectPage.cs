using System;
using System.IO;
using SimpleFileBrowser;

public class EditorFileSelectPage : EditorPageManager
{
    public override EditorPage EditorPage
    {
        get { return EditorPage.FileSelect; }
    }

    public string DefaultPath;
    private Action<string> _onFileSelectComplete;

    public void Show(string filePattern, Action<string> callback)
    {
        _onFileSelectComplete = callback;

        if (string.IsNullOrEmpty(filePattern))
        {
            filePattern = ".*";
        }

        var patterns = filePattern.Split(';');
        FileBrowser.SetFilters(true, patterns);
        FileBrowser.SetDefaultFilter(patterns[0]);

        foreach (var folder in Parent.CoreManager.Settings.GetResolvedSongFolders())
        {
            FileBrowser.AddQuickLink(folder, folder, null);
        }

        FileBrowser.ShowLoadDialog(FileSelectSuccess, FileSelectCancelled, FileBrowser.PickMode.Files, false, DefaultPath);
    }

    private void FileSelectSuccess(string[] paths)
    {
        _onFileSelectComplete(paths[0]);
    }

    private void FileSelectCancelled()
    {
        _onFileSelectComplete(null);
    }
}