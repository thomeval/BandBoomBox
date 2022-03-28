using System;
using UnityEngine;


[Serializable]
public class SongChart
{
    public string Group;
    public Difficulty Difficulty;
    public int DifficultyLevel;
    

    [HideInInspector]
    public string[] Notes;

    public override string ToString()
    {
        return string.Format("{0} - {1}({2})", Group, Difficulty, DifficultyLevel);
    }
}

