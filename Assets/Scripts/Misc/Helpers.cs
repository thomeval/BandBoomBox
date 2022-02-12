using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public static class Helpers
{
    private static Dictionary<string, string> _specialPaths = new Dictionary<string, string>()
    {
        {"%AppInstallFolder%", AppInstallFolder},
        {"%AppSaveFolder%", AppSaveFolder},
        {"%StreamingAssetsFolder%", Application.streamingAssetsPath}
    };

    /// <summary>
    /// Clamps the provided value to be between 0 and the given [max] value (inclusive).
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="max">The maximum value to clamp to (inclusive).</param>
    /// <returns>0 if [value] is less than 0, [max] if value is greater than [max], otherwise returns [value].</returns>
    public static int Clamp(int value, int max)
    {
        return Clamp(value, 0, max);
    }

    /// <summary>
    /// Clamps the provided value to be between the given [min] and [max] values (inclusive).
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value to clamp to (inclusive).</param>
    /// <param name="max">The maximum value to clamp to (inclusive).</param>
    /// <returns>[min] if [value] is less than [min], [max] if value is greater than [max], otherwise returns [value].</returns>
    public static int Clamp(int value, int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException("Min value is greater than max value.");
        }
        return Math.Max(min, Math.Min(max, value));
    }

    /// <summary>
    /// Wraps the provided value to be between the given [min] and [max] values (inclusive).
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="min">The minimum value to wrap to (inclusive).</param>
    /// <param name="max">The maximum value to wrap to (inclusive).</param>
    /// <returns>If [value] is between [min] and [max], then [value] will be returned. Otherwise it will be wrapped first, then returned.</returns>
    public static int Wrap(int value, int max)
    {
        if (max == 0)
        {
            return 0;
        }
        else if (max < 0)
        {
            throw new ArgumentException("Max value cannot be less than 0.");
        }
        while (value > max)
        {
            value -= (max + 1);
        }

        while (value < 0)
        {
            value += (max + 1);
        }

        return value;
    }

    /// <summary>
    /// Adds the provided amount to the provided enum, and returns the enum associated with the resulting integer.
    /// If the result is outside of the enum's valid values, it will be either clamped or wrapped, based on the [wrap] parameter.
    /// </summary>
    /// <param name="enumToAdd">The enum to be added.</param>
    /// <param name="delta">The amount to add (or subtract) to the provided enum.</param>
    /// <param name="wrap">If true, the result will be wrapped. Otherwise it will be clamped.</param>
    /// <returns>The enum  associated with the resulting sum. It will be clamped or wrapped, depending on the [wrap] parameter, if necessary.</returns>
    public static TimingDisplayType EnumAdd(TimingDisplayType enumToAdd, int delta, bool wrap)
    {
        var max = Enum.GetValues(typeof(TimingDisplayType)).Length - 1;

        var result = (int) enumToAdd;
        result += delta;

        if (wrap)
        {
            result = Wrap(result, max);
        }
        else
        {
            result = Clamp(result, max);
        }

        return (TimingDisplayType) result;
    }

    /// <summary>
    /// Adds the provided amount to the provided enum, and returns the enum associated with the resulting integer.
    /// If the result is outside of the enum's valid values, it will be either clamped or wrapped, based on the [wrap] parameter.
    /// </summary>
    /// <param name="enumToAdd">The enum to be added.</param>
    /// <param name="delta">The amount to add (or subtract) to the provided enum.</param>
    /// <param name="wrap">If true, the result will be wrapped. Otherwise it will be clamped.</param>
    /// <returns>The enum  associated with the resulting sum. It will be clamped or wrapped, depending on the [wrap] parameter, if necessary.</returns>
    public static Difficulty EnumAdd(Difficulty enumToAdd, int delta, bool wrap)
    {
        var max = Enum.GetValues(typeof(Difficulty)).Length - 1;

        var result = (int)enumToAdd;
        result += delta;

        if (wrap)
        {
            result = Wrap(result, max);
        }
        else
        {
            result = Clamp(result, max);
        }

        return (Difficulty) result;
    }

    public static float[] GradePercentages = 
    {
        0.96f, 0.93f, 0.9f, // SS, S+ and S grades
        0.85f, 0.8f,        // A Grades
        0.75f, 0.7f,        // B Grades
        0.65f, 0.6f,        // C Grades
        0.55f, 0.5f,        // D Grades
        0.4f                // E grade. Below this for F grade.
    };

    public static Grade PercentToGrade(float percent)
    {
        for (int x = 0; x < GradePercentages.Length; x++)
        {
            if (GradePercentages[x] <= percent)
            {
                return (Grade)x;
            }
        }

        return Grade.F;
    }

    public static float? GradeToPercent(Grade? grade)
    {
        if (grade == null)
        {
            return null;
        }
        if (grade == Grade.F)
        {
            return 0.0f;
        }
        return GradePercentages[(int) grade];
    }

    public static string PathToUri(string path)
    {
        return "file://" + path.Replace("\\", "/");
    }

    public static string ResolvePath(string path)
    {
        foreach (var specialPath in _specialPaths)
        {
            path = path.Replace(specialPath.Key, specialPath.Value);
        }

        path = CleanPathSeparators(path);
        return path;
    }

    public static string AppInstallFolder
    {
        get
        {
            string appFileName = Environment.GetCommandLineArgs()[0];
            return Path.GetDirectoryName(appFileName);
        }
    }
    public static string AppSaveFolder
    {
        get
        {
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrEmpty(myDocs) || !Directory.Exists(myDocs))
            {
                Debug.LogWarning($"MyDocuments folder is missing or invalid. Location: [{myDocs}]");
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games",
                "Band BoomBox");
        }
    }

    public static string AppLogsFolder
    {
        get
        {
            var appSubfolder = "DefaultCompany\\RGPR_2021";
            
            string result;
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "AppData\\LocalLow\\", appSubfolder);
                    break;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    result = Path.Combine("~/.config/unity3d/", appSubfolder);
                    break;
                default:
                    return null;
            }

            result = CleanPathSeparators(result);
            return result;
        }
    }

    public static void OpenFolderWindow(string folder)
    {
        folder = CleanPathSeparators(folder);


        if (!Directory.Exists(folder))
        {
            Debug.LogWarning($"Attempted to open folder at {folder} but it does not exist.");
            return;
        }

        switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    Process.Start("explorer", folder);
                    break;
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor: 
                    Process.Start("mimeopen", folder);
                break;
            }
    }

    private static string CleanPathSeparators(string folder)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return folder.Replace("/", "\\");
            default:
                return folder.Replace("\\", "/");
        }
    }

    public static T GetNextValue<T>(T[] array, T value, int delta, bool wrap)
    {
        var idx = Array.IndexOf(array, value);
        if (idx == -1)
        {
            throw new ArgumentException($"Value {value} is not in the provided array.");
        }

        idx += delta;
        if (wrap)
        {
            idx = Helpers.Wrap(idx, array.Length-1);
        }
        else
        {
            idx = Helpers.Clamp(idx, array.Length-1);
        }

        return array[idx];
    }

    #region Extension Methods
    public static GameObject[] GetChildren(this GameObject gameObject)
    {
        var childCount = gameObject.transform.childCount;
        var result = new GameObject[childCount];

        for (int x = 0; x < childCount; x++)
        {
            result[x] = gameObject.transform.GetChild(x).gameObject;
        }

        return result;
    }

    public static void AddChild(this GameObject gameObject, GameObject child)
    {
        if (gameObject == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        child.transform.SetParent(gameObject.transform, false);
    }

    public static void ClearChildren(this GameObject gameObject)
    {
        foreach (var child in gameObject.GetChildren())
        {
            Object.Destroy(child);
        }
    }

    public static void Hide(this MonoBehaviour behaviour)
    {
        behaviour.gameObject.SetActive(false);
    }

    public static void Show(this MonoBehaviour behaviour)
    {
        behaviour.gameObject.SetActive(true);
    }


    public static string AsPercent(this float value, int decimals = 0)
    {
        var formatString = "{0:P" + decimals + "}";
        return string.Format(CultureInfo.InvariantCulture, formatString, value).Replace(" %","%");
    }

    public static void PlayUnlessNull(this AudioSource source)
    {
        if (source.clip != null)
        {
            source.Play();
        }
    }
#endregion

}

