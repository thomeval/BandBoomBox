using System;
using Newtonsoft.Json;

[Serializable]
public class SongStarScoreValues
{
    public TeamScoreCategory ScoreCategory;
    public long[] Scores = new long[1];

    public long this[int idx]
    {
        get { return this.Scores[idx]; }
    }

    [JsonIgnore]
    public int Length
    {
        get { return this.Scores.Length; }
    }

    public double GetStarFraction(long score)
    {

        double result = 0.0;
        int star = 0;
        for (star = 0; star < this.Length; star++)
        {
            if (this[star] > score)
            {
                break;
            }

            result++;
        }

        if (star == this.Length)
        {
            return result;
        }

        var prev = star == 0 ? 0 : this[star-1];
        var next = this[star];

        var fraction = GetLerpFraction(score, prev, next);
        result += fraction;
        return result;
    }

    private double GetLerpFraction(long target, long left, long right)
    {
        target -= left;
        right -= left;
        if (right == 0)
        {
            throw new ArgumentException("Invalid LERP: left and right side are equal.");
        }
        return 1.0 * target / right;
    }
    public string Validate()
    {
        double last = -1.0;
        foreach (var score in Scores)
        {

            if (score <= 0)
            {
                return "Star score values must be greater than zero.";
            }
            if (score <= last)
            {
                return "Star score values must be in ascending order, and cannot contain duplicates.";
            }
        }

        return null;
    }
}

