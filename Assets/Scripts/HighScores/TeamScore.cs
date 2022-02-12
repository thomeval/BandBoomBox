using System;

[Serializable]
public class TeamScore
{
    public string SongId { get; set; }
    public string SongTitle { get; set; }
    public string SongArtist { get; set; }

    public int SongVersion { get; set; }


    public DateTime DateTime { get; set; }
    public int MaxTeamCombo { get; set; }
    public long Score { get; set; }
    public double MaxMultiplier { get; set; }

    public int NumPlayers { get; set; }

    public TeamScoreCategory Category { get; set; }
    public double Stars { get; set; }
}
