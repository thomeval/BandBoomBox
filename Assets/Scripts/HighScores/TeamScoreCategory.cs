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
    /// Represents a result for three or four players.
    /// </summary>
    Crowd = 4,

    /// <summary>
    /// Represents a result for five or more players.
    /// </summary>
    Legion = 5
}