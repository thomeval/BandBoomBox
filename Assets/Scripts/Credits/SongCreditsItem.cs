using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SongCreditsItem : MonoBehaviour
{
    public Text TxtArtistName;
    public Text TxtWebsiteUrl;
    public SpriteResolver IcnExtraUrl;
    public Text TxtExtraUrl;
    public Text TxtSongs;
    public GameObject SongsLineObject;

    private const int LINE_HEIGHT = 32;
    public void SetFromArtist(SongCreditsArtist artist)
    {
        TxtArtistName.text = artist.Name;
        TxtWebsiteUrl.text = artist.WebsiteUrl;
        TxtExtraUrl.text = artist.ExtraUrl;
        IcnExtraUrl.SetCategoryAndLabel("ExtraUrlIcons", artist.ExtraUrlType.ToString());

        var extraUrlVisible = artist.ExtraUrlType != SongCreditsUrlType.None;

        TxtExtraUrl.gameObject.SetActive(extraUrlVisible);
        IcnExtraUrl.gameObject.SetActive(extraUrlVisible);

        TxtSongs.text = "";

        foreach (var song in  artist.Songs.OrderBy(e => e))
        {
            TxtSongs.text += "- " + song + "\r\n";
        }

        var rt = SongsLineObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 30 + (artist.Songs.Count * LINE_HEIGHT));
    }

}
