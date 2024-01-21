public enum TeamScoreCategory
{
    /// <summary>
    /// Represents a result with no human players.
    /// </summary>
    NoPlayers = 0,

    /// <summary>
    /// Represents a result for a single player.
    /// </summary>
    Solo = 1,

    /// <summary>
    /// Represents a result for two players.
    /// </summary>
    Duet = 2,

    /// <summary>
    /// Not used. Use Squad for three players.
    /// </summary> 
    Trio = 3,

    /// <summary>
    /// Represents a result for three or four players.
    /// </summary>
    Squad = 4,

    /// <summary>
    /// Represents a result for five to eight players.
    /// </summary>
    Crowd = 5,

    /// <summary>
    /// Represents a result for nine or more players.
    /// 
    Legion = 9,
}