using System;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationSectionItem : MonoBehaviour
{
    public Text LblSectionName;
    public Text TxtSectionPercent;
    public Text TxtSectionGrade;

    public void Display(string sectionName, double percent, SectionJudgeMode mode)
    {
        var sectionGrade = Helpers.PercentToSectionGrade(percent, mode);
        var color = Helpers.SectionGradeToColor(sectionGrade);
        var percentStr = percent < 0.0f ? "--" : Math.Floor(100 * percent) + "%";
        LblSectionName.text = sectionName;
        TxtSectionPercent.text = percentStr;
        TxtSectionPercent.color = color;
        TxtSectionGrade.color = color;
        TxtSectionGrade.text = sectionGrade == SectionJudgeResult.Empty ? "NA" : sectionGrade.ToString();
    }

}
