
using UnityEngine;
using UnityEngine.UI;

public class ExpMeter : MonoBehaviour
{

    public Text TxtLevel;
    public SpriteMeter ProgressMeter;

    [SerializeField]
    private long _exp;

    public long Exp
    { 
        get
        {
            return _exp;
        }
        set
        {
            _exp = value;
            DisplayExp();
        }
    }

    private void DisplayExp()
    {
        var level = ExpLevelUtils.GetLevel(Exp);
        var progressPerc = ExpLevelUtils.GetProgressPercent(Exp);

        TxtLevel.text = string.Format("{0:00}", level);
        ProgressMeter.Value = progressPerc;
    }
}
