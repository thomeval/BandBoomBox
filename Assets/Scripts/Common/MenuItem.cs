using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{

    public bool IsSelectable;

    public bool IsShiftable;

    public string Text;

    public void SetText(string value)
    {
        Text = value;
        var textComponent = GetComponent<Text>();
        textComponent.text = Text;
    }

    [Multiline]
    public string Explanation;
}
