using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomBindingDisplayItem : MonoBehaviour
{
    public string Action;
    public string[] Bindings;
    public Text TxtBindings;
    public Color NormalTextColor = Color.white;
    public Color ErrorColor = Color.red;

    private CustomBindingDisplay _parentDisplay;

    private void Awake()
    {
        Helpers.AutoAssign(ref _parentDisplay);
    }

    public void Display(CustomBindingSet bindings)
    {        
        var myBindings = bindings.GetBindingsByAction(Action);
        Bindings = myBindings.Select(b => b.Path).ToArray();
        
        // Update Text
        if (TxtBindings != null)
        {
            if (Bindings.Length == 0)
            {
                TxtBindings.text = "[Not Bound!]";
                TxtBindings.color = ErrorColor;
                return;
            }
            var displayBindings = myBindings.Select(b => ConvertPath(b.Path)).ToArray();
            TxtBindings.text = string.Join(", ", displayBindings);
            TxtBindings.color = NormalTextColor;
        }
    }

    public void AddBinding()
    {
        _parentDisplay.ListenForNewBinding(this.Action);
    }

    public void ClearBindings()
    {
        _parentDisplay.ClearBindings(this.Action);
    }

    public string ConvertPath(string path)
    {
        return path.Replace("<Keyboard>/", "")
            .Replace("Arrow","");
    }
}
