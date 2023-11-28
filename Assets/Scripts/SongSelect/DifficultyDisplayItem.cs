using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class DifficultyDisplayItem : MonoBehaviour
{
    public SpriteMeter Layer1Sprite;
    public SpriteMeter Layer2Sprite;

    public Text TxtDifficultyText;
    public Text TxtDifficultyLevels;

    public Difficulty Difficulty;
    public SpriteResolver GradeSprite;

    private readonly Dictionary<Difficulty, Color> _layer1Colors = new Dictionary<Difficulty, Color>
    {
        {Difficulty.Beginner, new Color(0.0f, 0.33f, 1.0f )},
        {Difficulty.Medium, new Color(0.0f, 0.5f, 0.0f )},
        {Difficulty.Hard, new Color(0.65f, 0.65f, 0.0f )},
        {Difficulty.Expert, new Color(0.8f, 0.0f, 0.0f )},
        {Difficulty.Nerf, new Color(0.65f, 0.0f, 0.65f )},
        {Difficulty.Extra, new Color(0.0f, 0.6f, 0.6f )},
    };

    private readonly Dictionary<Difficulty, Color> _layer2Colors = new Dictionary<Difficulty, Color>
    {
        {Difficulty.Beginner, new Color(0.6f, 0.7f, 1.0f )},
        {Difficulty.Medium, new Color(0.6f, 1.0f, 0.6f )},
        {Difficulty.Hard, new Color(1.0f, 1.0f, 0.6f )},
        {Difficulty.Expert, new Color(1.0f, 0.65f, 0.65f )},
        {Difficulty.Nerf, new Color(1.0f, 0.7f, 1.0f )},
        {Difficulty.Extra, new Color(0.0f, 0.9f, 0.9f )},
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

    public void DisplayDifficulty(Difficulty difficulty, int level)
    {
        DisplayDifficulty(new DifficultyRange { Difficulty = difficulty, Min = level, Max = level });
    }

    public void DisplayDifficulty(DifficultyRange diff)
    {
        var visible = !diff.IsEmpty;
        this.gameObject.SetActive(visible);

        this.Difficulty = diff.Difficulty;
        TxtDifficultyText.text = diff.Difficulty.GetDisplayName();

        if (_menuItem != null)
        {
            _menuItem.Text = diff.ToString();
        }

        Layer1Sprite.InnerSprite.color = _layer1Colors[Difficulty];
        Layer2Sprite.InnerSprite.color = _layer2Colors[Difficulty];

        if (diff.Min == diff.Max)
        {
            TxtDifficultyLevels.text = string.Format("{0:00}", diff.Min);
        }
        else
        {
            TxtDifficultyLevels.text = string.Format("{0:00}-{1:00}", diff.Min, diff.Max);
        }

        UpdateDifficultyBar(diff.Max);
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

    public void DisplayGrade(Grade? grade)
    {
        var label = grade == null ? "None" : grade.ToString();

        GradeSprite.SetCategoryAndLabel("Grades", label);
    }
}
