using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class HighScoreManager : MonoBehaviour
{

    private PlayerManager _playerManager;

    public List<TeamScore> TeamScores = new List<TeamScore>();

    public string TeamHighScoresPath = "%AppSaveFolder%/TeamScores.bin";

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    public void Load()
    {
        var path = Helpers.ResolvePath(TeamHighScoresPath);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Team high scores file was not found at {path}");
            return;
        }

        try
        {
            var json = CompressedFileHelper.DecompressFromFile(path);
            var temp = JsonConvert.DeserializeObject<List<TeamScore>>(json);
            TeamScores = temp.ToList();
            Debug.Log($"Team high scores successfully loaded from {path}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load team high scores: {e}");
        }
    }

    public void Save()
    {
        try
        {
            var path = Helpers.ResolvePath(TeamHighScoresPath);

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
        var category = GetCategory(playerCount);

        var result = TeamScores.SingleOrDefault(e => e.Category == category && e.SongId == songId && e.SongVersion == songVersion);
        return result;
    }

    public bool AddTeamScore(TeamScore teamScore)
    {
        var existing = GetTeamScore(teamScore.SongId, teamScore.NumPlayers, teamScore.SongVersion);

        if (existing == null)
        {
            TeamScores.Add(teamScore);
            Save();
            return true;
        }
        if (existing.Score >= teamScore.Score)
        {
            return false;
        }

        TeamScores.Remove(existing);
        TeamScores.Add(teamScore);
        Save();
        return true;
    }

    public static TeamScoreCategory GetCategory(int playerCount)
    {
        if (playerCount >= 5)
        {
            return TeamScoreCategory.Legion;
        }

        if (playerCount >= 3)
        {
            return TeamScoreCategory.Crowd;
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
}
