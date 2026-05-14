using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SecretCodeListEntry : MonoBehaviour
{
    [SerializeField]
    private string _codeName;
    [SerializeField]
    private InputAction[] _codeSequence;

    public Text TxtCodeName;
    public GameObject CodeSequenceSpriteContainer;
    public GameObject IconPrefab;

    public void DisplayCode(string codeName, InputAction[] codeSequence)
    {
        _codeName = codeName;
        _codeSequence = codeSequence;

        TxtCodeName.text = codeName;
        CodeSequenceSpriteContainer.ClearChildren();

        foreach (var action in _codeSequence) 
        {
            var icon = Instantiate(IconPrefab, CodeSequenceSpriteContainer.transform);
            var resolver = icon.GetComponent<SpriteResolver>();
            var actionStr = action == InputAction.Pause ? "Start" : action.ToString();
            resolver.SetCategoryAndLabel("ButtonPrompts", actionStr);
        }
    }
}