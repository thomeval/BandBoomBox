using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class CustomBindingDisplayItem : MonoBehaviour
{
    public string Action;
    public string[] Bindings;
    public Text TxtBindings;

    public void Display(CustomBindingSet bindings)
    {
        var myBindings = bindings.GetBindingsByAction(Action);
        Bindings = myBindings.Select(b => ConvertPath(b.Path)).ToArray();
        TxtBindings.text = string.Join(", ", Bindings);
    }

    public string ConvertPath(string path)
    {
        return path.Replace("<Keyboard>/", "");
    }
}
