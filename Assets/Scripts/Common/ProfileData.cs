using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class ProfileData
{
    public string ID { get; set; }
    public string Name { get; set; }
    public TimingDisplayType TimingDisplayType { get; set; }
    public int ScrollSpeed { get; set; }
    public long Exp { get; set; }

    public float? Goal { get; set; }
    public Difficulty Difficulty { get; set; }

    public int SongsPlayed { get; set; }
    public bool MistakeSfxEnabled { get; set; } = true;

    public List<PlayerScore> PlayerScores { get; set; } = new();

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


    public void ApplyToPlayer(Player player)
    {
        player.ProfileId = this.ID;
        player.Name = this.Name;
        player.Exp = this.Exp;
        player.ScrollSpeed = this.ScrollSpeed;
        player.TimingDisplayType = this.TimingDisplayType;
        player.Goal = this.Goal;
        player.Difficulty = this.Difficulty;
        player.SongsPlayed = this.SongsPlayed;
        player.MistakeSfxEnabled = this.MistakeSfxEnabled;
    }

    public PlayerScore GetBestPlayerHighScore(string songId, int songVersion)
    {
        var scores = PlayerScores.Where(e => e.SongId == songId && e.SongVersion == songVersion)
            .OrderByDescending(e => e.PerfPoints)
            .ToList();

        return scores.FirstOrDefault();
    }
}

