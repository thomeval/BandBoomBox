using System.Globalization;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSongFrame : MonoBehaviour
{
    public Text TxtTitle;
    public Text TxtSubtitle;
    public Text TxtArtist;
    public Text TxtBpm;
    public Text TxtAuthor;
    public Text TxtLength;
    public SongIssuesDisplay SongIssuesDisplay;

    [SerializeField]
    private SongData _selectedSong;
    public SongData SelectedSong
    {
        get { return _selectedSong; }
        set
        {
            _selectedSong = value;
            Refresh();
        }

    }

    private void Refresh()
    {
        if (SelectedSong != null)
        {
            TxtTitle.text = SelectedSong.Title;
            TxtSubtitle.text = SelectedSong.Subtitle;
            TxtArtist.text = SelectedSong.Artist;
            TxtBpm.text = string.Format(CultureInfo.InvariantCulture, "{0:N1}", SelectedSong.Bpm);
            TxtLength.text = string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}", (int) (SelectedSong.Length / 60),
                SelectedSong.Length % 60);
            TxtAuthor.text = SelectedSong.ChartAuthor;
            SongIssuesDisplay.SongData = SelectedSong;
        }

    }

}
