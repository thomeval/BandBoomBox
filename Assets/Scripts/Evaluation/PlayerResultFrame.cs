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
    public ExpModifierList ExpModifierList;

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

    public Text TxtAccuracy;
    public Text TxtDeviation;

    [Header("Common")]
    public ExpMeter ExpMeter;

    public SpriteResolver PlayerIdentifier;
    public GameObject[] Pages;
    public bool DisplayAllPages;

    [SerializeField]
    private int _displayedPage;
    public int DisplayedPage
    {
        get { return _displayedPage; }
        set
        {
            _displayedPage = Helpers.Wrap(value, Pages.Length-1);
            DisplayPage(_displayedPage);
        }
    }

    private Animator _animator;

    private float _totalExpModifier
    {
        get
        {
            return ExpModifierList.TotalExpModifier;
        }
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void DisplayPage(int pageNum)
    {
        if (DisplayAllPages)
        {
            foreach (var page in Pages)
            {
                page.SetActive(true);
            }
            return;
        }

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
    public void DisplayResult(Player player, bool isNewPb, double stars, int numPlayers)
    {
        this.gameObject.SetActive(true);
        TxtPlayerName.text = player.Name;
        TxtDifficulty.text = player.Difficulty.ToString();


        TxtIsNewPb.gameObject.SetActive(isNewPb);
        var grade = player.GetCurrentGrade().ToString();
        GradeSprite.SetCategoryAndLabel("Grades",grade);
        TxtMaxCombo.text = string.Format("{0:000}", player.MaxCombo);
        TxtPercentage.text = string.Format(CultureInfo.InvariantCulture, "{0:P1}", player.PerfPercent);
        PlayerIdentifier.SetCategoryAndLabel("PlayerIdentifiers", player.GetPlayerIdSprite());
     
        DisplayHitCount(player);
        DisplayAccuracyDeviation(player);
        ExpModifierList.DisplayExpModifier(player, stars, numPlayers);

        var totalExpGain = ExpModifierList.GetTotalExpGain(player);
        ExpMeter.Exp = totalExpGain;
        TxtExpGain.text = string.Format("+{0} EXP", totalExpGain);
        var isLevelUp = ExpLevelUtils.IsLevelUp(player.Exp, totalExpGain);
        TxtIsLevelUp.gameObject.SetActive(isLevelUp);

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

    private void DisplayAccuracyDeviation(Player player)
    {
      //  TxtAccuracy.text = FormatMilliseconds(player.HitAccuracyAverage);
        TxtDeviation.text = FormatMilliseconds(player.HitDeviationAverage);
    }

    private string FormatMilliseconds(float seconds)
    {
        var suffix = seconds < 0.0f ? "Early" : "Late";
        var amount = Math.Abs(seconds * 1000);
        return $"{(amount):f1} ms {suffix}";
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

