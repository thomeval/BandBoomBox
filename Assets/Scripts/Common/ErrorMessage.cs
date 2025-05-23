using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ErrorMessage : MonoBehaviour
{
    public Color ErrorTextColor = new Color(1.0f, 0.5f, 0.5f);
    public Color DefaultTextColor = Color.white;

    public GameObject Background;
    private Text _txtMessage;

    public string DefaultMessage = "";
    public string Error
    {
        get { return _txtMessage.text == DefaultMessage ? null : _txtMessage.text; }
        set
        {
            var isError = !string.IsNullOrEmpty(value);

            if (isError)
            {
                _txtMessage.text = value;
                _txtMessage.color = ErrorTextColor;
            }
            else
            {
                _txtMessage.text = DefaultMessage;
                _txtMessage.color = DefaultTextColor;
            }

            if (Background != null)
            {
                Background.SetActive(!string.IsNullOrEmpty(_txtMessage.text));
            }
        }
    }

    void OnEnable()
    {
        _txtMessage = GetComponent<Text>();
    }

    void Awake()
    {
        _txtMessage = GetComponent<Text>();
    }

    void Start()
    {
       // Error = null;
    }
}
