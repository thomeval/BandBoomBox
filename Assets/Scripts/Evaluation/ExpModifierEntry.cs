using UnityEngine;
using UnityEngine.UI;

public class ExpModifierEntry : MonoBehaviour
{
    public Text TxtLabel;
    public Text TxtValue;

    [SerializeField]
    private float _value;
    public float Value
    {
        get
        {
            return _value;
        }
    }

    public Color PositiveValueColor = Color.white;
    public Color NegativeValueColor = new Color(1.0f,0.5f,0.5f, 1.0f);

    public void Display(string label, float value)
    {
        TxtLabel.text = label;
        _value = value;
        TxtValue.text = FormatAsExp(value);

        var color = value >= 1.0f ? PositiveValueColor : NegativeValueColor;
        TxtLabel.color = color;
        TxtValue.color = color;
    }

    private string FormatAsExp(float amount)
    {
        amount--;
        var prefix = amount < 0 ? "" : "+";
        var result = $"{prefix}{amount:P0} EXP".Replace(" %", "");
        return result;
    }

}
