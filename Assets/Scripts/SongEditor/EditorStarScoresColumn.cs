using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class EditorStarScoresColumn : MonoBehaviour
{

    public TeamScoreCategory TeamScoreCategory;
    public SpriteResolver CategoryIconSprite;

    public InputField[] TextBoxes = new InputField[5];

    public SongStarScoreValues ScoreValues
    {
        get
        {
            return GetValues();
        }
        set
        {
            SetValues(value);
        }
    }

    private SongStarScoreValues GetValues()
    {
        var values = new long[TextBoxes.Length];

        for (int x = 0; x < values.Length; x++)
        {
            if (long.TryParse(TextBoxes[x].text, out long value))
            {
                values[x] = value;
            }

        }

        var result = new SongStarScoreValues { ScoreCategory = this.TeamScoreCategory, Scores = values };
        return result;
    }

    private void SetValues(SongStarScoreValues values)
    {
        this.TeamScoreCategory = values.ScoreCategory;
        CategoryIconSprite.SetCategoryAndLabel("ScoreCategories", TeamScoreCategory.ToString());

        var length = Math.Min(values.Length, TextBoxes.Length);
        for (int x = 0; x < length; x++)
        {
            TextBoxes[x].text = "" + values[x];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (CategoryIconSprite != null)
        {
            CategoryIconSprite.SetCategoryAndLabel("ScoreCategories", TeamScoreCategory.ToString());
        }
    }

    public void Clear()
    {
        for (int x = 0; x < TextBoxes.Length; x++)
        {
            TextBoxes[x].text = "0";
        }
    }

    public string Validate()
    {
        var result = "";

        for (int x = 0; x < ScoreValues.Length; x++)
        {
            if (ScoreValues[x] < 0)
            {
                result = "Negative values are not allowed.";
                break;
            }

            if (x > 0 && ScoreValues[x - 1] > ScoreValues[x])
            {
                result = "Each score requirement must be bigger than the value above it.";
                break;
            }
        }

        if (result == "")
        {
            return "";
        }

        return TeamScoreCategory + " column: " + result;
    }
}
