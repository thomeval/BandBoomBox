using UnityEngine;

public class EditorPageManager : MonoBehaviour
{
    private EditorManager _parent;

    public EditorManager Parent
    {
        get
        {
            _parent ??= FindObjectOfType<EditorManager>();
            return _parent;
        }
    }
    public bool IgnoreReleaseInputs;
    public virtual EditorPage EditorPage
    {
        get { return EditorPage.None; }
    }

    public void HandleInput(InputEvent inputEvent, bool isRelease)
    {
        if (IgnoreReleaseInputs && isRelease)
        {
            return;
        }
        HandleInput(inputEvent);
    }

    public virtual void HandleInput(InputEvent inputEvent)
    {

    }

}

