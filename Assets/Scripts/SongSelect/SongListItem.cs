﻿using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SongListItem : MonoBehaviour
{
    public Text TxtTitle;
    public Text TxtArtist;
    public Text TxtStarCount;
    public GameObject StarCountDisplay;

    public SpriteResolver FrameSpriteResolver;
    public SpriteResolver StarIconSpriteResolver;

    public void DisplayTeamScore(TeamScore teamScore)
    {
        var starsText = teamScore == null ? "" : ((int)teamScore.Stars) + "";
        var label = teamScore == null ? "None" : starsText;

        TxtStarCount.text = starsText;
        StarIconSpriteResolver.SetCategoryAndLabel("StarIcons", label);
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

    private void ResolveSprite()
    {
        var label = _isSelected ? "Selected" : "NotSelected";
        FrameSpriteResolver.SetCategoryAndLabel("SongListItem", label);
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

