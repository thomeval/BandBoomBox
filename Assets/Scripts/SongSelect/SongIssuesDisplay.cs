using UnityEngine;
using UnityEngine.UI;

public class SongIssuesDisplay : MonoBehaviour
{
    private SongData _songData;
    public SongData SongData
    {
        get { return _songData; }
        set
        {
            _songData = value;
            ShowIssues();
        }
    }

    public Text TxtIssues;

    private void ShowIssues()
    {
        if (_songData == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.gameObject.SetActive(!string.IsNullOrEmpty(_songData.Issues));
        TxtIssues.text = _songData.Issues;
    }
}