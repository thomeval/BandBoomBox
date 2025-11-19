using System;
using UnityEngine;
using UnityEngine.UI;

public class EvaluationSectionList : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemContainer;

    [SerializeField]
    private ScrollRect _containerScrollRect;

    public GameObject SectionItemPrefab;

    public int ScrollSpeed = 30;

    public void DisplaySections(string[] sectionNames, double[] percentages, SectionJudgeMode mode)
    {
        _itemContainer.ClearChildren();

        var itemCount = Math.Max(sectionNames.Length, percentages.Length);
        for (int i = 0; i < itemCount; i++)
        {    
            var sectionName = (i >= sectionNames.Length) ? "Unknown" : sectionNames[i];
            var value = (i >= percentages.Length) ? -1 : percentages[i];
            var itemObj = Instantiate(SectionItemPrefab, this.transform);
            var item = itemObj.GetComponent<EvaluationSectionItem>();
            item.Display(sectionName, value, mode);
        }
    }

    public void Scroll(int direction)
    {
        if (_containerScrollRect != null)
        {
            _containerScrollRect.MoveVerticalInUnits(direction * ScrollSpeed);
        }
    }

}
