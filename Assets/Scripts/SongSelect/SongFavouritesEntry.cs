using System.Linq;

public class SongFavouritesEntry
{
    public string SongId;
    public bool[] IsLocalFavorite = new bool[8];
    public int RemoteFavoriteCount;

    public int TotalCount => RemoteFavoriteCount + IsLocalFavorite.Count(fav => fav);

}