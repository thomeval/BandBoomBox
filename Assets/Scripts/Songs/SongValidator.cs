using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public static class SongValidator
{

    public static string Validate(SongData songData, bool allowZeroCharts = false)
    {
        songData.Sections ??= new Dictionary<double,string>();
        songData.SongCharts ??= new List<SongChart>();

        var result = "";

        if (string.IsNullOrWhiteSpace(songData.ID))
        {
            result += "Song ID cannot be empty;";
        }

        if (songData.Bpm < 1.0f || songData.Bpm > 999.0f)
        {
            result += "BPM must be between 1 and 999.;";
        }

        if (string.IsNullOrWhiteSpace(songData.Title))
        {
            result += "Song Title cannot be empty;";
        }
        if (string.IsNullOrWhiteSpace(songData.Artist))
        {
            result += "Song Artist cannot be empty;";
        }
        if (string.IsNullOrWhiteSpace(songData.ChartAuthor))
        {
            result += "Chart Author cannot be empty;";
        }

        if (songData.AudioStart < 0.0f)
        {
            result += "Audio Start cannot be negative.;";
        }
        if (songData.Offset < 1.0f)
        {
            result += "Offset must be at least 1.0 seconds.;";
        }
        if (songData.Length < 10.0f)
        {
            result += "Length must be at least 10.0 seconds.;";
        }

        if (songData.AudioStart > songData.Offset)
        {
            result += "Offset time must be greater than Audio Start time.;";
        }
        if (songData.Offset > songData.Length)
        {
            result += "Length time must be greater than Offset time.;";
        }


        if (!allowZeroCharts && !songData.SongCharts.Any())
        {
            result += "This song contains no charts. It will not be playable without one!;";
        }

        var groups = songData.SongCharts.GroupBy(e => e.Group + "-" + e.Difficulty);

        foreach (var duplicate in groups.Where(g => g.Count() > 1))
        {
            result += $"Multiple charts found with group and difficulty: {duplicate.Key};";
        }

        result = result.Replace(";", "\r\n");

        return result;
    }
}
