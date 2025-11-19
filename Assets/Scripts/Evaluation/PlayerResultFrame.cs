using System;
using System.Linq;
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
    public Text TxtRanking;
    public Text LblMaxCombo;
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

    public Text LblAccuracyDeviation;
    public Text TxtAccuracy;
    public Text TxtDeviation;
    public Text TxtBoosts;

    [Header("Page 3")]
    public EvaluationSectionList SectionList;

    [Header("Common")]
    public ExpMeter ExpMeter;

    public SpriteResolver PlayerIdentifier;
    public GameObject[] Pages;
    public bool DisplayAllPages;
    public GameObject ReadyPage;
    public int PlayerSlot;
    public SoundEventProvider SoundEventProvider;

    [SerializeField]
    private int _displayedPage;
    public int DisplayedPage
    {
        get { return _displayedPage; }
        set
        {
            _displayedPage = Helpers.Wrap(value, Pages.Length - 1);
            DisplayPage(_displayedPage);
        }
    }

    [SerializeField]
    private bool _showDeviation = true;
    public bool ShowDeviation
    {
        get { return _showDeviation; }
        set
        {
            _showDeviation = value;
            LblAccuracyDeviation.text = _showDeviation ? "Timing" : "Accuracy";
            TxtAccuracy.gameObject.SetActive(!value);
            TxtDeviation.gameObject.SetActive(value);
        }
    }

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        Helpers.AutoAssign(ref SoundEventProvider);
    }
    private void DisplayPage(int pageNum)
    {
        ReadyPage.SetActive(false);

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

    public void DisplayResult(Player player, bool isNewPb, double stars, int numPlayers, string[] sectionNames)
    {
        this.gameObject.SetActive(true);
        this.PlayerSlot = player.Slot;
        TxtPlayerName.text = player.Name;
        TxtDifficulty.text = player.GroupAndDifficulty;

        TxtIsNewPb.gameObject.SetActive(isNewPb);
        var grade = player.GetCurrentGrade().ToString();
        GradeSprite.SetCategoryAndLabel("Grades", grade);

        var fullComboType = player.GetFullComboType();
        if (LblMaxCombo != null)
        {
            LblMaxCombo.text = ComboUtils.GetFcCode(fullComboType);
            LblMaxCombo.color = ComboUtils.GetFcColor(fullComboType);
        }
        TxtMaxCombo.color = ComboUtils.GetFcColor(fullComboType);
        TxtMaxCombo.text = string.Format("{0:000}", player.MaxCombo);

        TxtPercentage.text = Helpers.FormatPercent(player.PerfPercent);
        TxtRanking.text = Helpers.FormatRanking(player.Ranking);
        PlayerIdentifier.SetCategoryAndLabel("PlayerIdentifiers", player.GetPlayerIdSprite());

        DisplayHitCount(player);
        DisplayAccuracyDeviation(player);
        if (TxtBoosts != null)
        {
            TxtBoosts.text = string.Format("P:{0}, R:{1}, E:{2}", player.AllyBoostsProvided, player.AllyBoostsReceived, player.AllyBoosts);
        }
        ExpModifierList.DisplayExpModifier(player, stars, numPlayers);

        var totalExpGain = ExpModifierList.GetTotalExpGain(player);
        ExpMeter.Exp = player.Exp + totalExpGain;
        TxtExpGain.text = string.Format("+{0} EXP", totalExpGain);
        var isLevelUp = ExpLevelUtils.IsLevelUp(player.Exp, totalExpGain);
        TxtIsLevelUp.gameObject.SetActive(isLevelUp);

        DisplayGradeAnimation();
        DisplaySectionList(sectionNames, player.SectionPercents.ToArray(), player.ProfileData.SectionDifficulty);
    }

    private void DisplaySectionList(string[] sectionNames, double[] sectionPercents, SectionJudgeMode mode)
    {
        SectionList.DisplaySections(sectionNames, sectionPercents, mode);
    }

    public void DisplayReady()
    {
        foreach (var page in Pages)
        {
            page.SetActive(false);
        }
        ReadyPage.SetActive(true);
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
        TxtAccuracy.text = $"{player.HitAccuracyAverage * 1000:f1} ms";
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
            return string.Format("{0:0000}", player.EarlyHits[judgeResult] + player.LateHits[judgeResult]);
        }

        return string.Format("{0:0000}|{1:0000}", player.EarlyHits[judgeResult], player.LateHits[judgeResult]);

    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void PlaySfx(SoundEvent soundEvent)
    {
        SoundEventProvider.PlaySfx(soundEvent, PlayerSlot);
    }

    public void Scroll(int direction)
    {
        if (SectionList.isActiveAndEnabled)
        {
            SectionList.Scroll(direction);
        }
    }
}

