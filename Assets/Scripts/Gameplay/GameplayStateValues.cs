using UnityEngine;

public class GameplayStateValues : MonoBehaviour
{
    public float SongPosition;

    public long Score;
    public int TeamCombo;
    public int MaxTeamCombo;

    public double Multiplier = 1;
    public double MaxMultiplier = 1;

    public float MxGainRate = 1;

    public double Energy;
    public double MaxEnergy;

    public TeamScoreCategory TeamScoreCategory;
    public double Stars;

    public GameplayStateValuesDto AsDto()
    {
        return new GameplayStateValuesDto
        {
            SongPosition = SongPosition,
            Score = Score,
            TeamCombo = TeamCombo,
            MaxTeamCombo = MaxTeamCombo,
            Multiplier = Multiplier,
            MaxMultiplier = MaxMultiplier,
            MxGainRate = MxGainRate,
            Energy = Energy,
            MaxEnergy = MaxEnergy,
            TeamScoreCategory = TeamScoreCategory,
            Stars = Stars
        };
    }

    public void CopyValues(GameplayStateValuesDto dto)
    {
        SongPosition = dto.SongPosition;
        Score = dto.Score;
        TeamCombo = dto.TeamCombo;
        MaxTeamCombo = dto.MaxTeamCombo;
        Multiplier = dto.Multiplier;
        MaxMultiplier = dto.MaxMultiplier;
        MxGainRate = dto.MxGainRate;
        Energy = dto.Energy;
        MaxEnergy = dto.MaxEnergy;
        TeamScoreCategory = dto.TeamScoreCategory;
        Stars = dto.Stars;
    }
}
