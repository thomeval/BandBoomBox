using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public static class ProjectBuilder
{
    private static string _outputFolder = "D:\\Writable Folder\\Band BoomBox";

    public static string[] ActiveScenePaths
    {
        get
        {
            var result = new List<string>();

            for (int x = 0; x < SceneManager.sceneCount; x++)
            {
                result.Add(SceneManager.GetSceneAt(x).path);
            }

            return result.ToArray();
        }
    }
    public static void BuildAll()
    {
        BuildProject(Path.Combine(_outputFolder, "Windows\\BandBoomBox.exe"), BuildTarget.StandaloneWindows64);
        BuildProject(Path.Combine(_outputFolder, "Linux\\BandBoomBox.x86_64"), BuildTarget.StandaloneLinux64);
    }

    public static void BuildProject(string path, BuildTarget buildTarget)
    {
        var options = new BuildPlayerOptions
        {
            scenes = ActiveScenePaths.ToArray(), target = buildTarget, locationPathName = path,
        };

        BuildPipeline.BuildPlayer(options);
    }


}
