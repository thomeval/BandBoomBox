
public static class ExpLevelUtils
{
    private static readonly long[] _expLevels =
    {
        0, 500, 1000, 1500, 2000, 3000, 4000, 5000, 6500, 8000,                             // 1 - 10
        10000, 12000, 14000, 16500, 19000, 21500, 24000, 27000, 30000, 35000,               // 11 - 20
        40000, 46000, 52000, 59000, 66000, 74000, 83000, 93000, 104000, 116000,             // 21 - 30
        129000, 143000, 158000, 175000, 195000, 215000, 237000, 259000, 283000, 309000,     // 31 - 40
        337000, 367000, 400000, 436000, 475000, 517000, 562000, 610000, 661000, 715000      // 41 - 50
    };

    public static int GetLevel(long exp)
    {
        for (int x = 0; x < _expLevels.Length; x++)
        {
            if (_expLevels[x] > exp)
            {
                return x;
            }
        }

        return _expLevels.Length;
    }

    public static float GetProgressPercent(long exp)
    {
        var level = GetLevel(exp);

        if (level == _expLevels.Length)
        {
            return 1.0f;
        }

        var nextLevelExp = _expLevels[level ] - _expLevels[level-1];
        var currentExp = exp - _expLevels[level-1];

        return 1.0f * currentExp / nextLevelExp;
    }

    public static long GetExpToNextLevel(long exp)
    {
        var level = GetLevel(exp);

        if (level == _expLevels.Length)
        {
            return 0;
        }

        return _expLevels[level + 1] - exp;
    }

    public static bool IsLevelUp(long currentExp, long awardedExp)
    {
        var level = GetLevel(currentExp);

        if (level == _expLevels.Length)
        {
            return false;
        }

        return currentExp + awardedExp >= _expLevels[level];
    }
}
