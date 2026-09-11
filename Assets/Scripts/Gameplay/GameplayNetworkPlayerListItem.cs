using UnityEngine;
using UnityEngine.UI;

public class GameplayNetworkPlayerListItem : NetworkPlayerListItem
{
    [Header("Gameplay")]
    public Text TxtChartGroupDifficulty;
    public Text TxtPerfPercent;
    public Text TxtCombo;
    public Text TxtRanking;
    public GameObject TurboBackground;
    public SectionResultDisplay SectionResultDisplay;
    public AllyBoostStatusDisplay AllyBoostStatusDisplay;
    public static Color AutoPlayTextColor = new Color (0.8f, 0.0f, 0.0f); 
    public static Color NormalTextColor = Color.black;

    private AllyBoostManager _allyBoostManager;

    public override void Refresh()
    {
        base.Refresh();

        if (_allyBoostManager == null)
        {
            Helpers.AutoAssign(ref _allyBoostManager);
        }

        var playerColor = Player.AutoPlayEnabled ? AutoPlayTextColor : NormalTextColor;
        SetTextColorSafe(TxtPlayerName, playerColor);
        SetTextColorSafe(TxtChartGroupDifficulty, playerColor);

        if (TxtChartGroupDifficulty != null)
        {
            if (Player.PlayerState != PlayerState.Gameplay_Playing)
            {
                TxtChartGroupDifficulty.text = GetStatusText(Player.PlayerState);
            }
            else if (Player.ChartGroup == "Main")
            {
                TxtChartGroupDifficulty.text = $"{Player.Difficulty}";
            }
            else
            {
                TxtChartGroupDifficulty.text = $"{Player.ChartGroup} - {Player.Difficulty}";
            }
        }

        SetTextSafe(TxtCombo, $"{Player.Combo:000}");
        SetTextColorSafe (TxtCombo, playerColor);

        if (!Player.IsParticipating)
        {
            SetTextSafe(TxtRanking, "-");
            SetTextSafe(TxtPerfPercent, "");
            AllyBoostStatusDisplay.gameObject.SetActive(false);
        }
        else
        {
            SetTextSafe(TxtRanking, Helpers.FormatRanking(Player.Ranking));
            SetTextSafe(TxtPerfPercent, Helpers.FormatPercent(Player.PerfPercent));
            AllyBoostStatusDisplay.UpdateDisplay(_allyBoostManager.GetPlayer(Player.NetId, Player.Slot));
            AllyBoostStatusDisplay.gameObject.SetActive(true);
        }

        SetTextColorSafe(TxtRanking, playerColor);
        SetTextColorSafe(TxtPerfPercent, playerColor);

        if (TurboBackground != null)
        {
            TurboBackground.SetActive(Player.TurboActive);
        }
    }

    public override void DisplaySectionResult(SectionResultDto sectionResult)
    {
        Debug.Log($"Player {Player.DisplayNetId}{Player.Slot} Section Result: {sectionResult.SectionAccuracy:P0}({sectionResult.JudgeResult})");
        SectionResultDisplay.ShowSectionResult(sectionResult.JudgeResult);
    }

    public override void DisplayFullComboResult(FullComboResultDto result)
    {
        Debug.Log($"Player {Player.DisplayNetId}{Player.Slot} Full Combo Result: {result.FullComboType}");

        if (result.FullComboType != FullComboType.None)
        {
            SectionResultDisplay.ShowFullComboResult(result.FullComboType);
        }

    }

    public override void SetIndex(int index)
    {
        base.SetIndex(index);
        if (SectionResultDisplay != null)
        {
            SectionResultDisplay.SetIndex(index);
        }
    }
}
