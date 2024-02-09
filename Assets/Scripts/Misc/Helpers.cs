using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Helpers
{
    private static readonly Dictionary<string, string> _specialPaths = new()
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
    /// Wraps the provided value to be between 0 and [max] value (inclusive).
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="max">The maximum value to wrap to (inclusive).</param>
    /// <returns>If [value] is between 0 and [max], then [value] will be returned. Otherwise it will be wrapped first, then returned.</returns>
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

        return (TimingDisplayType)result;
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

        return (Difficulty)result;
    }

    /// <summary>
    /// Adds the provided amount to the provided enum, and returns the enum associated with the resulting integer.
    /// If the result is outside of the enum's valid values, it will be either clamped or wrapped, based on the [wrap] parameter.
    /// </summary>
    /// <param name="enumToAdd">The enum to be added.</param>
    /// <param name="delta">The amount to add (or subtract) to the provided enum.</param>
    /// <param name="wrap">If true, the result will be wrapped. Otherwise it will be clamped.</param>
    /// <returns>The enum  associated with the resulting sum. It will be clamped or wrapped, depending on the [wrap] parameter, if necessary.</returns>
    public static T EnumAdd<T>(T enumToAdd, int delta, bool wrap)
        where T : Enum
    {

        var max = Enum.GetValues(typeof(T)).Length - 1;

        var result = Convert.ToInt32(enumToAdd);
        result += delta;
        Enum.ToObject(typeof(T), result);
        if (wrap)
        {
            result = Wrap(result, max);
        }
        else
        {
            result = Clamp(result, max);
        }

        return (T)Enum.ToObject(typeof(T), result);
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
        return GradePercentages[(int)grade];
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
            var appSubfolder = "thomeval\\Band BoomBox";

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

    public static bool OpenFolderWindow(string folder)
    {
        folder = CleanPathSeparators(folder);

        if (!Directory.Exists(folder))
        {
            Debug.LogWarning($"Attempted to open folder at {folder} but it does not exist.");
            return false;
        }

        Process.Start(new ProcessStartInfo { FileName = folder, UseShellExecute = true, Verb = "open" });
        return true;
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

    /// <summary>
    /// Finds the index of the given <c>value</c> in the given array, adds <c>delta</c> to the result, then returns the element located at that index.
    /// For example, if delta is -1, then this method will return the element before the matching element.
    /// If multiple elements match the provided value, then the first match is used.
    /// If the index of the matching element + <c>delta</c> exceeds the array bounds, the index will be clamped to be within the array's bounds, unless <c>wrap</c> is set to true.
    /// Throws ArgumentException if <c>value</c> does not exist in <c>array</c>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the array, and the value to search for.</typeparam>
    /// <param name="array">The array within which to search for <c>value</c>.</param>
    /// <param name="value">The value to search for in the provided array.</param>
    /// <param name="delta">The delta to add to the index of the matching element. Negative numbers are allowed.</param>
    /// <param name="wrap">Whether the resulting index should wrap around if it exceeds the array bounds. If set to false, the resulting index is clamped instead.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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
            idx = Helpers.Wrap(idx, array.Length - 1);
        }
        else
        {
            idx = Helpers.Clamp(idx, array.Length - 1);
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
            UnityEngine.Object.Destroy(child);
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

    public static void ToggleVisibility(this MonoBehaviour behaviour)
    {
        if (behaviour.gameObject.activeSelf)
        {
            behaviour.gameObject.SetActive(false);
            return;
        }
        behaviour.gameObject.SetActive(true);
    }

    public static string AsPercent(this float value, int decimals = 0)
    {
        var formatString = "{0:P" + decimals + "}";
        return string.Format(CultureInfo.InvariantCulture, formatString, value).Replace(" %", "%");
    }

    public static void PlayUnlessNull(this AudioSource source)
    {
        if (source.clip != null)
        {
            source.Play();
        }
    }

    public static void AutoAssign<T>(ref T component)
        where T : MonoBehaviour
    {
        if (component == null)
        {
            component = UnityEngine.Object.FindObjectOfType<T>();
        }
    }
    #endregion

    public static T TryGetArg<T>(Dictionary<string, object> args, string key)
    {
        if (!args.ContainsKey(key) || args[key] is not T)
        {
            return default(T);
        }

        return (T)args[key];
    }

    public static string GetDisplayName(this Difficulty diff)
    {
        switch (diff)
        {
            case Difficulty.Nerf:           // Not Even Remotely Fair
                return "N.E.R.F.";
            default:
                return diff.ToString();

        }
    }

    public static Difficulty GetDifficultyByDisplayName(string displayName)
    {
        switch (displayName)
        {
            case "N.E.R.F.":
                return Difficulty.Nerf;

            default:
                var valid = Enum.TryParse(displayName, out Difficulty temp);
                if (!valid)
                {
                    throw new ArgumentException($"Invalid difficulty name: {displayName}");
                }
                return temp;
        }

    }

    public static string FormatRanking(int ranking)
    {
        string suffix;
        switch (ranking)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
            default:
                suffix = "th";
                break;
        }

        return $"{ranking}{suffix}";
    }

    public static string FormatPercent(float value)
    {
        var result = value.ToString("P1", CultureInfo.InvariantCulture);
        return result.Replace(" %", "%");
    }

    public static string NumberToNetIdLetter(ulong netId)
    {
        if (netId == 0)
        {
            return "A";
        }

        if (netId == 26)
        {
            return "AA";
        }

        var result = "";
        while (netId > 0)
        {
            var remainder = netId % 26;
            result = (char)('A' + remainder) + result;
            netId /= 26;
        }

        return result;
    }
}

