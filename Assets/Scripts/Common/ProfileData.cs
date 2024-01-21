﻿using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class ProfileData
{
    public string ID { get; set; }
    public string Name { get; set; }
    public TimingDisplayType TimingDisplayType { get; set; }
    public int ScrollSpeed { get; set; }
    public long Exp { get; set; }
    public float? Goal { get; set; }
    public int SongsPlayed { get; set; }
    public bool MistakeSfxEnabled { get; set; } = true;
    public bool RumbleEnabled { get; set; } = true;
    public bool SeenNerfWarning { get; set; } = false;

    public List<PlayerScore> PlayerScores { get; set; } = new();
    public int Momentum { get; set; }

    public PlayerScore GetPlayerHighScore(string songId, int songVersion, Difficulty difficulty, string chartGroup)
    {
        return PlayerScores.SingleOrDefault(e =>
            e.SongId == songId && e.SongVersion == songVersion && e.Difficulty == difficulty && e.ChartGroup == chartGroup);
    }

    public bool AddPlayerScore(PlayerScore playerScore)
    {
        var existing = GetPlayerHighScore(playerScore.SongId, playerScore.SongVersion, playerScore.Difficulty, playerScore.ChartGroup);

        if (existing == null)
        {
            PlayerScores.Add(playerScore);
            return true;
        }
        if (existing.PerfPercent >= playerScore.PerfPercent)
        {
            return false;
        }

        PlayerScores.Remove(existing);
        PlayerScores.Add(playerScore);
        return true;
    }

    public PlayerScore GetBestPlayerHighScore(string songId, int songVersion)
    {
        var scores = PlayerScores.Where(e => e.SongId == songId && e.SongVersion == songVersion)
            .OrderByDescending(e => e.PerfPoints)
            .ToList();

        return scores.FirstOrDefault();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}

