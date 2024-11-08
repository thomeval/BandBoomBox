using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{

    public List<TeamScore> TeamScores = new();

    private readonly string _teamHighScoresPath = "%AppSaveFolder%/TeamScores.bin";

    public void Load()
    {
        var path = Helpers.ResolvePath(_teamHighScoresPath);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Team high scores file was not found at {path}");
            return;
        }

        try
        {
            var json = CompressedFileHelper.DecompressFromFile(path);
            var temp = JsonConvert.DeserializeObject<List<TeamScore>>(json);
            TeamScores = temp;
            Debug.Log($"Team high scores successfully loaded from {path}. Loaded {TeamScores.Count} entries.");

            FixDuplicates();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load team high scores: {e}");
        }
    }

    public void FixDuplicates()
    {
        var dups = TeamScores.GroupBy(e => $"{e.SongVersion} {e.SongId} {e.Category}").Where(g => g.Count() > 1);
        var scoresToRemove = new List<TeamScore>();

        foreach (var dupGroup in dups)
        {
            var duds = dupGroup.OrderByDescending(e => e.Score).Skip(1).ToList();
            scoresToRemove.AddRange(duds);
        }

        scoresToRemove.AddRange(TeamScores.Where(e => e.Category == TeamScoreCategory.NoPlayers).ToList());
        var count = scoresToRemove.Count;

        foreach (var score in scoresToRemove)
        {
            TeamScores.Remove(score);
        }

        if (count == 0)
        {
            return;
        }

        Debug.Log($"Removed {count} invalid/duplicate team scores.");
        Save();

    }

    public void Save()
    {
        try
        {
            var path = Helpers.ResolvePath(_teamHighScoresPath);

            // Built in Unity Json serializer doesn't support arrays or collections. Using Json.Net instead.
            var json = JsonConvert.SerializeObject(TeamScores);

            CompressedFileHelper.CompressToFile(path, json);
            Debug.Log($"Team high scores successfully saved to {path}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to save team high scores: {e}");
        }
    }


    public TeamScore GetTeamScore(string songId, int songVersion, int playerCount)
    {
        var category = GetScoreCategory(playerCount);

        try
        {
            var result = TeamScores.SingleOrDefault(e => e.Category == category && e.SongId == songId && e.SongVersion == songVersion);
            return result;
        }
        catch (InvalidOperationException ex)
        {
            Debug.LogError($"Failed to retrieve team high score for song {songId}, version {songVersion}, category {category}. {ex}");
            return null;
        }

    }

    public bool AddTeamScore(TeamScore teamScore)
    {
        if (teamScore.Category == TeamScoreCategory.NoPlayers)
        {
            return false;
        }

        var existing = GetTeamScore(teamScore.SongId, teamScore.SongVersion, teamScore.NumPlayers);

        if (existing == null)
        {
            Debug.Log($"Inserting Team High Score: {teamScore.SongId}, {teamScore.Category}, v{teamScore.SongVersion} : {teamScore.Score}");
            TeamScores.Add(teamScore);
            Save();

            Debug.Assert(GetTeamScoreCount(teamScore.SongId, teamScore.Category, teamScore.SongVersion) == 1);
            return true;
        }
        if (existing.Score >= teamScore.Score)
        {
            return false;
        }

        Debug.Log($"Updating Team High Score: {teamScore.SongId}, {teamScore.Category}, v{teamScore.SongVersion} : {teamScore.Score}");
        TeamScores.Remove(existing);
        TeamScores.Add(teamScore);
        Save();

        Debug.Assert(GetTeamScoreCount(teamScore.SongId, teamScore.Category, teamScore.SongVersion) == 1);
        return true;
    }

    private int GetTeamScoreCount(string songId, TeamScoreCategory category, int songVersion)
    {
        return TeamScores.Count(e => e.SongId == songId && e.Category == category && e.SongVersion == songVersion);
    }

    public static TeamScoreCategory GetScoreCategory(int playerCount)
    {
        if (playerCount >= 9)
        {
            return TeamScoreCategory.Legion;
        }
        if (playerCount >= 5)
        {
            return TeamScoreCategory.Crowd;
        }
        if (playerCount >= 3)
        {
            return TeamScoreCategory.Squad;
        }
        if (playerCount == 2)
        {
            return TeamScoreCategory.Duet;
        }
        if (playerCount == 1)
        {
            return TeamScoreCategory.Solo;
        }

        return TeamScoreCategory.NoPlayers;
    }

    public int GetTotalStarsForSongs(List<SongData> songs, int playerCount)
    {
        var stars = songs.Select(e => GetTeamScore(e.ID, e.Version, playerCount))
            .Where(e => e != null)
            .Select(e => (int)e.Stars)
            .Sum();
        return stars;
    }
}
