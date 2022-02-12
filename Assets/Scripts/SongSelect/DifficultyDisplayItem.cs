using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDisplayItem : MonoBehaviour
{
    public SpriteMeter Layer1Sprite;
    public SpriteMeter Layer2Sprite;

    public Text TxtDifficultyText;
    public Text TxtDifficultyLevels;

    public Difficulty Difficulty;

    private readonly Dictionary<Difficulty, Color> _layer1Colors = new Dictionary<Difficulty, Color>
    {
        {Difficulty.Beginner, new Color(0.0f, 0.33f, 1.0f )},
        {Difficulty.Medium, new Color(0.0f, 0.5f, 0.0f )},
        {Difficulty.Hard, new Color(0.65f, 0.65f, 0.0f )},
        {Difficulty.Expert, new Color(0.8f, 0.0f, 0.0f )},
        {Difficulty.Master, new Color(0.65f, 0.0f, 0.65f )},
    };

    private readonly Dictionary<Difficulty, Color> _layer2Colors = new Dictionary<Difficulty, Color>
    {
        {Difficulty.Beginner, new Color(0.6f, 0.7f, 1.0f )},
        {Difficulty.Medium, new Color(0.6f, 1.0f, 0.6f )},
        {Difficulty.Hard, new Color(1.0f, 1.0f, 0.6f )},
        {Difficulty.Expert, new Color(1.0f, 0.65f, 0.65f )},
        {Difficulty.Master, new Color(1.0f, 0.7f, 1.0f )},
    };

    private MenuItem _menuItem;

    void Awake()
    {
        _menuItem = GetComponent<MenuItem>();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void DisplayDifficulty(Difficulty diff, int minLevel, int maxLevel)
    {
        var visible = (minLevel > -1 || maxLevel > -1);
        this.gameObject.SetActive(visible);

        this.Difficulty = diff;
        TxtDifficultyText.text = diff.ToString();

        if (_menuItem != null)
        {
            _menuItem.Text = diff.ToString();
        }

        Layer1Sprite.InnerSprite.color = _layer1Colors[Difficulty];
        Layer2Sprite.InnerSprite.color = _layer2Colors[Difficulty];

        if (minLevel == maxLevel)
        {
            TxtDifficultyLevels.text = string.Format("{0:00}", minLevel);
        }
        else
        {
            TxtDifficultyLevels.text = string.Format("{0:00}-{1:00}", minLevel, maxLevel);
        }

        UpdateDifficultyBar(maxLevel);
    }

    private void UpdateDifficultyBar(int maxLevel)
    {
        // First Layer
        var value = Mathf.Clamp(maxLevel / 10.0f, 0.0f, 1.0f);
        Layer1Sprite.Value = value;

        // Second Layer
        maxLevel -= 10;
        value = Mathf.Clamp(maxLevel / 10.0f, 0.0f, 1.0f);
        Layer2Sprite.Value = value;
    }
}
