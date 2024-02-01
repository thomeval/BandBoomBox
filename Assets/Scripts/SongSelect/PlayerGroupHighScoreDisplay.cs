using UnityEngine;

public class PlayerGroupHighScoreDisplay : MonoBehaviour
{
    public PlayerHighScoreDisplay[] PlayerHighScoreDisplays;

    private PlayerManager _playerManager;
    private ProfileManager _profileManager;

    void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _profileManager = FindObjectOfType<ProfileManager>();

    }

    public void FetchHighScores(string songId, int songVersion)
    {
        ShowHighScoreDisplays();
        for (int x = 0; x < PlayerHighScoreDisplays.Length; x++)
        {
            FetchHighScore(x, songId, songVersion);
        }
    }

    private void ShowHighScoreDisplays()
    {
        if (_playerManager == null)
        {
            Awake();
        }

        for (int x = 0; x < PlayerHighScoreDisplays.Length; x++)
        {
            var display = PlayerHighScoreDisplays[x];

            display.Clear();
            var player = _playerManager.GetPlayer(x +1);

            if (player != null)
            {
                display.Show();
            }
            else
            {
                display.Hide();
            }
        }
    }

    private void FetchHighScore(int slot, string songId, int songVersion)
    {
        var display = PlayerHighScoreDisplays[slot];
        var player = _playerManager.GetPlayer(slot+1);
        if (player == null)
        {
            return;
        }

        var bestScore = _profileManager.GetBestPlayerHighScore(player.ProfileId, songId, songVersion);
        display.DisplayedScore = bestScore;

    }

}
