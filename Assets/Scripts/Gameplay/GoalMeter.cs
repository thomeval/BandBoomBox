using System;
using UnityEngine;
using UnityEngine.UI;

public class GoalMeter : MonoBehaviour
{

    [SerializeField]
    private int _min;
    public int Min
    {
        get { return _min; }
        set
        {
            _min = value;
            Refresh();
        }
    }

    [SerializeField]
    private int _max;
    public int Max
    {
        get { return _max; }
        set
        {
            _max = value;
            Refresh();
        }
    }

    [SerializeField]
    private int _value;
    public int Value
    {
        get { return _value; }
        set
        {
            _value = value;
            Refresh();
        }
    }

    [SerializeField]
    private int _pace;
    public int Pace
    {
        get { return _pace; }
        set
        {
            _pace = value;
            Refresh();
        }
    }

    [SerializeField]
    private int _playerCurrent;
    public int PlayerCurrent
    {
        get { return _playerCurrent; }
        set
        {
            _playerCurrent = value;
            Refresh();
        }
    }

    [SerializeField]
    private Grade? _goalGrade;
    public Grade? GoalGrade
    {
        get { return _goalGrade; }
        set
        {
            _goalGrade = value;
            Refresh();
        }
    }

    public SpriteMeter Meter;
    public Text TxtGoalGrade;

    public GameObject FailSprite;
    public GameObject ClearSprite;
    public GameObject DangerSprite;

    [Range(0.0f,1.0f)]
    public float BeatPulseAmount = 0.3f;

    public float DangerPercent = 0.2f;

    private SongManager _songManager;

    private void Refresh()
    {
        if (this.GoalGrade == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        

        this.gameObject.SetActive(true);

        TxtGoalGrade.text = this.GoalGrade?.ToString().Replace("Plus", "+");

        UpdateMeter();
        DisplaySprite();
    }

    private void DisplaySprite()
    {
        FailSprite.SetActive(false);
        ClearSprite.SetActive(false);
        DangerSprite.SetActive(false);

        if (PlayerCurrent >= Min)
        {
            ClearSprite.SetActive(true);
        }
        else if (Value < Min)
        {
            FailSprite.SetActive(true);
        }
        else
        {
            var meterPercent = GetMeterPercent();

            if  (meterPercent < DangerPercent)
            {
                DangerSprite.SetActive(true);
            }
        }
    }

    private float GetMeterPercent()
    {
        var tempMax = Max - Min;
        var tempValue = Value - Min;
        tempValue = Math.Max(0, tempValue);
        var result = 1.0f * tempValue / tempMax;

        return result;
    }

    private void UpdateMeter()
    {
        if (Max == 0)
        {
            return;
        }

        var percent = GetMeterPercent();

        var beat = _songManager.GetSongPositionInBeats();
        beat -= Mathf.Floor(beat);
        beat *= BeatPulseAmount;
        percent *= 1 - beat;
        Meter.Value = percent;

    }

    void Awake()
    {
        _songManager = FindObjectOfType<SongManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMeter();
    }
}
