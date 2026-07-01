using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SongListItem : MonoBehaviour
{
    public Text TxtTitle;
    public Text TxtArtist;
    public Text TxtStarCount;
    public GameObject StarCountDisplay;
    public SongListItemFavouriteDisplay FavouriteDisplay;

    public SpriteResolver FrameSpriteResolver;
    public SpriteResolver StarIconSpriteResolver;

    public Color DefaultColor = Color.white;
    public Color HasIssuesColor = Color.red;
    public Color NotSelectableColor = Color.gray;

    public void DisplayTeamScore(TeamScore teamScore)
    {
        var starsText = teamScore == null ? "" : ((int)teamScore.Stars) + "";
        var label = teamScore == null ? "None" : starsText;

        TxtStarCount.text = starsText;
        StarIconSpriteResolver.SetCategoryAndLabel("StarIcons", label);
    }

    public void DisplayFavorites(SongFavouritesEntry favouritesEntry)
    {
        FavouriteDisplay.Display(favouritesEntry);
    }

    [SerializeField]
    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
            ResolveSprite();
        }
    }

    [SerializeField]
    private bool _isSelectable = true;
    public bool IsSelectable
    {
        get
        {
            return _isSelectable;
        }
        set
        {
            _isSelectable = value;
            ResolveSprite();
        }
    }

    private void ResolveSprite()
    {
        var label = _isSelected ? "Selected" : "NotSelected";
        FrameSpriteResolver.SetCategoryAndLabel("SongListItem", label);
        var color = GetTextColor();
        TxtArtist.color = color;
        TxtTitle.color = color;
    }

    private Color GetTextColor()
    {
        if (!IsSelectable)
        {
            return NotSelectableColor;
        }
        if (!string.IsNullOrEmpty(SongData.Issues))
        {
            return HasIssuesColor;
        }
        return DefaultColor;
    }

    [SerializeField]
    private SongData _songData;

    public SongData SongData
    {
        get { return _songData; }
        set
        {
            _songData = value;
            DisplaySongData();
        }
    }

    private void DisplaySongData()
    {
        TxtTitle.text = SongData.Title;
        TxtArtist.text = SongData.Artist;
        ResolveSprite();
    }

}

