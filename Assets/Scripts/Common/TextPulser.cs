using UnityEngine;
using UnityEngine.UI;

public class TextPulser : MonoBehaviour
{

    public Text Text;

    public float Period = 1.0f;

    [Range(0.0f, 1.0f)]
    public float Min = 0.0f;
    [Range(0.0f, 1.0f)]
    public float Max = 1.0f;

    private Color _color;
    // Start is called before the first frame update
    void Start()
    {
        _color = new Color(Text.color.r, Text.color.g, Text.color.b, Text.color.a);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.Period == 0.0f)
        {
            return;
        }

        var scale = Max - Min;

        unchecked
        {
            var y = Mathf.PingPong(Time.realtimeSinceStartup / Period, 1);
            y *= scale;
            y += this.Min;

            _color.a = y;
            Text.color = _color;
        }


    }
}
