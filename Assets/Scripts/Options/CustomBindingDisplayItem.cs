using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CustomBindingDisplayItem : MonoBehaviour
{
    public string Action;
    public string[] Bindings;
    public Text TxtBindings;
    
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
            var displayBindings = myBindings.Select(b => ConvertPath(b.Path)).ToArray();
            TxtBindings.text = string.Join(", ", displayBindings);
        }
    }

    public void AddBinding()
    {
        _parentDisplay.ListenForNewBinding(this.Action);
    }

    public string ConvertPath(string path)
    {
        return path.Replace("<Keyboard>/", "");
    }
}
