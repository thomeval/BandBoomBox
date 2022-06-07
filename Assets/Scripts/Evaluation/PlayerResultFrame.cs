using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerResultFrame : MonoBehaviour
{
    [Header("Page 1")]
    public Text TxtPlayerName;
    public Text TxtDifficulty;
    public Text TxtPercentage;
    public SpriteResolver GradeSprite;
    public Text TxtMaxCombo;
    public Text TxtExpGain;
    public Text TxtIsLevelUp;
    public Text TxtIsNewPb;
    public Text LblIsGoalMet;
    public Text TxtIsGoalMet;
    public Text LblIsFullCombo;
    public Text TxtIsFullCombo;

    [Header("Page 2")]
    public Text TxtCrit;
    public Text TxtCritPerc;
    public Text TxtPerfect;
    public Text TxtPerfectPerc;
    public Text TxtCool;
    public Text TxtCoolPerc;
    public Text TxtOk;
    public Text TxtOkPerc;
    public Text TxtBad;
    public Text TxtBadPerc;
    public Text TxtMiss;
    public Text TxtMissPerc;
    public Text TxtWrong;

    [Header("Common")]
    public ExpMeter ExpMeter;


    public SpriteResolver PlayerIdentifier;
    public GameObject[] Pages;

    [SerializeField]
    private int _displayedPage;
    public int DisplayedPage
    {
        get { return _displayedPage; }
        set
        {
            _displayedPage = Helpers.Clamp(value, 0, Pages.Length-1);
            DisplayPage(_displayedPage);
        }
    }

    private Animator _animator;
    private readonly Color _goalMetColor = new Color(1.0f,1.0f,1.0f,1.0f);
    private readonly Color _goalFailedColor = new Color(1.0f,0.5f,0.5f, 1.0f);

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void DisplayPage(int pageNum)
    {
        if (pageNum < 0 || pageNum >= Pages.Length)
        {
            return;
        }

        foreach (var page in Pages)
        {
            page.SetActive(false);
        }

        Pages[pageNum].SetActive(true);
    }
    public void DisplayResult(Player player, bool isNewPb, bool isLevelUp)
    {
        this.gameObject.SetActive(true);
        TxtPlayerName.text = player.Name;
        TxtDifficulty.text = player.Difficulty.ToString();
        TxtIsLevelUp.gameObject.SetActive(isLevelUp);
        TxtIsNewPb.gameObject.SetActive(isNewPb);
        var grade = player.GetCurrentGrade().ToString();
        GradeSprite.SetCategoryAndLabel("Grades",grade);
        TxtMaxCombo.text = string.Format("{0:000}", player.MaxCombo);
        TxtExpGain.text = string.Format("+{0} EXP", player.GetExpGain());
        TxtPercentage.text = string.Format(CultureInfo.InvariantCulture, "{0:P1}", player.PerfPercent);
        PlayerIdentifier.SetCategoryAndLabel("PlayerIdentifiers", player.GetPlayerIdSprite());
        ExpMeter.Exp = player.Exp + player.GetExpGain();
        DisplayHitCount(player);
        DisplayGoalResult(player);
        DisplayFullComboResult(player);
        DisplayGradeAnimation();

    }

    private void DisplayGradeAnimation()
    {
        _animator.Play("GradeShown");
    }

    private void DisplayHitCount(Player player)
    {
        TxtCrit.text = FormatHits(player, JudgeResult.Crit);
        TxtPerfect.text = FormatHits(player, JudgeResult.Perfect);
        TxtCool.text = FormatHits(player, JudgeResult.Cool);
        TxtOk.text = FormatHits(player, JudgeResult.Ok);
        TxtBad.text = FormatHits(player, JudgeResult.Bad);
        TxtMiss.text = FormatHits(player, JudgeResult.Miss);
        TxtWrong.text = FormatHits(player, JudgeResult.Wrong);

        TxtCritPerc.text = $"{player.HitPercentage(JudgeResult.Crit):P0}";
        TxtPerfectPerc.text = $"{player.HitPercentage(JudgeResult.Perfect):P0}";
        TxtCoolPerc.text = $"{player.HitPercentage(JudgeResult.Cool):P0}";
        TxtOkPerc.text = $"{player.HitPercentage(JudgeResult.Ok):P0}";
        TxtBadPerc.text = $"{player.HitPercentage(JudgeResult.Bad):P0}";
        TxtMissPerc.text = $"{player.HitPercentage(JudgeResult.Miss):P0}";
    }

    private void DisplayGoalResult(Player player)
    {
        if (player.Goal == null)
        {
            LblIsGoalMet.Hide();
            TxtIsGoalMet.Hide();
            return;
        }

        LblIsGoalMet.Show();
        TxtIsGoalMet.Show();

        var goalMet = player.PerfPoints  >= player.GoalPerfPoints.GetValueOrDefault();
        var textColor = goalMet ? _goalMetColor : _goalFailedColor;
        LblIsGoalMet.color = textColor;
        TxtIsGoalMet.color = textColor;

        LblIsGoalMet.text = goalMet ? "Goal Passed!" : "Goal Failed!";

        var goalExpValue = goalMet ? HitJudge.GoalExpValues[player.GetGoalGrade().GetValueOrDefault()] : 0.5f;
        TxtIsGoalMet.text = ToExpModifier(goalExpValue);

    }

    private void DisplayFullComboResult(Player player)
    {
        var fullComboType = player.GetFullComboType();
        if (fullComboType == FullComboType.None)
        {
            LblIsFullCombo.Hide();
            TxtIsFullCombo.Hide();
            return;
        }

        LblIsFullCombo.Show();
        TxtIsFullCombo.Show();

        var fcExpValue = HitJudge.FullComboExpValues[fullComboType];
        LblIsFullCombo.text = GetFullComboLabel(fullComboType);
        TxtIsFullCombo.text = ToExpModifier(fcExpValue);
    }

    private string GetFullComboLabel(FullComboType fullComboType)
    {
        switch (fullComboType)
        {
            case FullComboType.FullCombo:
                return "Full Combo!";
            case FullComboType.PerfectFullCombo:
                return "Perfect FC!";
            default:
                return "";
        }
    }

    private string ToExpModifier(float amount)
    {
        amount--;
        var prefix = amount < 0 ? "" : "+";
        var result = $"{prefix}{amount:P0} EXP".Replace(" %", "");
        return result;
    }

    private string FormatHits(Player player, JudgeResult judgeResult)
    {

        if (judgeResult == JudgeResult.Miss || judgeResult == JudgeResult.Wrong)
        {
            return string.Format("{0:0000}", player.Mistakes[judgeResult]);
        }

        if (judgeResult == JudgeResult.Perfect || judgeResult == JudgeResult.Crit)
        {
            return string.Format("{0:0000}",player.EarlyHits[judgeResult] + player.LateHits[judgeResult]);
        }

        return string.Format("{0:0000}|{1:0000}", player.EarlyHits[judgeResult], player.LateHits[judgeResult]);

    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}

