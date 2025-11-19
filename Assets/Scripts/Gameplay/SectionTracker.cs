using UnityEngine;

public class SectionTracker : MonoBehaviour
{
    private SongManager _songManager;
    private GameplayManager _gameplayManager;

    [SerializeField]
    private int _lastSectionIndex = 0;

    private void Awake()
    {
        Helpers.AutoAssign(ref _songManager);
        Helpers.AutoAssign(ref _gameplayManager);
    }

    public void UpdateSection()
    {
        var idx = _songManager.GetCurrentSectionIndex();

        if (idx == -1)
        {
            return;
        }

        if (idx == _lastSectionIndex)
        {
            return;
        }

        _lastSectionIndex = idx;

        _gameplayManager.EndSection();
    }
}
