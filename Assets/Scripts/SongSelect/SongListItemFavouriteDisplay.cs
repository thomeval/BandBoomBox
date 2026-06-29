using UnityEngine;
using UnityEngine.UI;

public class SongListItemFavouriteDisplay : MonoBehaviour
{
    public GameObject[] LocalFavouriteIcons;
    public GameObject RemoteFavouriteContainer;
    public Text TxtRemoteFavouriteCount;
    public void Display(SongFavouritesEntry entry)
    {
        for (int i = 0; i < LocalFavouriteIcons.Length && i < entry.IsLocalFavorite.Length; i++)
        {
            LocalFavouriteIcons[i].SetActive(entry.IsLocalFavorite[i]);
        }

        RemoteFavouriteContainer.SetActive(entry.RemoteFavoriteCount > 0);
        TxtRemoteFavouriteCount.text = entry.RemoteFavoriteCount.ToString();
    }
}