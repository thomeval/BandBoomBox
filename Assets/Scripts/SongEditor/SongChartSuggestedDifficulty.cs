using System;
using System.Collections.Generic;

public static class SongChartSuggestedDifficulty
{
	private static readonly Dictionary<Difficulty, double[]> _npsPerDifficulty = new()
    {
        { Difficulty.Beginner, new double[] {	0.00, 1.205, 1.805, 2.505 } },
		{ Difficulty.Medium, new double[] {		0.00, 0.905, 1.305, 1.555, 1.905, 2.405, 3.305 } },
		{ Difficulty.Mild, new double[] {		0.00, 0.755, 1.005, 1.305, 1.805, 2.305, 2.805, 3.805 } },
		{ Difficulty.Hard, new double[] {		0.00, 0.000, 0.000, 1.005, 1.305, 1.605, 2.005, 2.605, 2.805, 3.305, 3.705, 4.505, 5.105 } },
		{ Difficulty.Expert, new double[] {		0.00, 0.000, 0.000, 0.000, 1.005, 1.105, 1.205, 1.405, 1.705, 2.005, 2.505, 3.105, 3.705, 4.205, 5.005, 5.505, 6.205 } },
	};


	public static int? GetSuggestedDifficulty(Difficulty difficulty, double nps)
	{
        if (nps < 0.01)
        {
            return null;
        }

		if (!_npsPerDifficulty.ContainsKey(difficulty))
		{
			return null;
		}
		var thresholds = _npsPerDifficulty[difficulty];
        for (int i = 0; i < thresholds.Length; i++)
		{
			if (nps <= thresholds[i])
			{
				return Math.Max(1, i);
			}
		}
		return thresholds.Length;
	}
}