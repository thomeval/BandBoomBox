using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Menu : MonoBehaviour
{

    private readonly List<MenuItem> _menuItems = new List<MenuItem>();

    [Header("Components")]
    public GameObject MenuItemsContainer;
    public MenuSelectionHighlight SelectionHighlight;
    public Vector2 SelectionHighlightPadding = new Vector2();
    public Text TxtExplanation;
    public float YScrollOffset = 0;
    private VerticalLayoutGroup _layoutGroup;
    private RectTransform _itemsContainerRectTransform;

    [Header("Sounds")]
    public AudioSource SfxSelectionShifted;
    public AudioSource SfxSelectionChanged;
    public AudioSource SfxSelectionConfirmed;
    public AudioSource SfxSelectionCancelled;

    [Header("Behaviour")]
    public bool WrapSelection;

    [SerializeField]
    private int _selectedIndex;
    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set
        {
            _selectedIndex = (value >= _menuItems.Count) ? 0 : value;
            MoveHighlight();
            UpdateExplanationText();
        }
    }

    public Vector2 MenuItemsBottom
    {
        get
        {
            if (!_menuItems.Any())
            {
                return new Vector2(0,0);
            }

            var rt = _menuItems.Last().GetComponent<RectTransform>();
            return ((Vector2) rt.localPosition) - rt.sizeDelta;
        }
    }
    public void UpdateExplanationText()
    {
        if (TxtExplanation != null && SelectedMenuItem != null)
        {
            TxtExplanation.text = SelectedMenuItem.Explanation;
        }
    }

    public MenuItem SelectedMenuItem
    {
        get
        {

            if (SelectedIndex >= _menuItems.Count)
            {
                return null;
            }

            var item = _menuItems[SelectedIndex];

            return item;
        }
    }

    public string SelectedText
    {
        get { return SelectedMenuItem?.Text; }
    }

    public GameObject SelectedGameObject
    {
        get { return SelectedMenuItem?.gameObject; }
    }

    public string CancelMenuAction;

    public int Player;

    public bool Enabled
    {
        get { return this.gameObject.activeSelf; }
        set
        {
            this.gameObject.SetActive(value);
        }
    }

    public bool SelectionHighlightVisibility
    {
        get { return SelectionHighlight.gameObject.activeSelf; }
        set
        {
            SelectionHighlight.SetActive(value);

            if (value)
            {
                ForceLayoutUpdate();
                MoveHighlight();
            }
        }
    }

    void Awake()
    {
        _menuItems.Clear();

        foreach (var item in MenuItemsContainer.GetChildren())
        {
            var menuItem = item.GetComponent<MenuItem>();

            if (menuItem == null)
            {
                throw new ArgumentException("Only GameObjects that include a MenuItem component can be added to Menus.");
            }
            _menuItems.Add(item.GetComponent<MenuItem>());
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        FullRefresh();
    }

    void OnEnable()
    {
        FullRefresh();
    }

    public void HighlightMenuItem(MenuItem item)
    {
        var idx = _menuItems.IndexOf(item);
        idx = Math.Max(idx, 0);
        this.SelectedIndex = idx;
    }

    public void HighlightMenuItem(string text)
    {
        var item = _menuItems.FirstOrDefault(e => e.Text == text);

        if (item == null)
        {
            SelectedIndex = 0;
        }
        else
        {
            HighlightMenuItem(item);
        }
    }
    public void FullRefresh()
    {
        ForceLayoutUpdate();
        MoveHighlight();
        UpdateExplanationText();
    }

    private void ForceLayoutUpdate()
    {
        if (_layoutGroup == null)
        {
            _layoutGroup = MenuItemsContainer.GetComponent<VerticalLayoutGroup>();
            _itemsContainerRectTransform = MenuItemsContainer.GetComponent<RectTransform>();
        }

        _layoutGroup.CalculateLayoutInputHorizontal();
        _layoutGroup.CalculateLayoutInputVertical();
        _layoutGroup.SetLayoutHorizontal();
        _layoutGroup.SetLayoutVertical();
    }

    private void MoveHighlight()
    {
        if (SelectedGameObject == null)
        {
            return;
        }

        YScrollOffset = CalculateYOffset();
        MenuItemsContainer.transform.localPosition = new Vector3(0, YScrollOffset, 0);
        SelectionHighlight.Padding = this.SelectionHighlightPadding;
        SelectionHighlight.HighlightObject(SelectedGameObject);
    }

    private float CalculateYOffset()
    {
        var preferredHeight = _layoutGroup.preferredHeight;
        var actualHeight = this.GetComponent<RectTransform>().sizeDelta.y;
        var diff = preferredHeight - actualHeight;

        if (diff <= 0 || SelectedGameObject == null)
        {
            return 0;
        }

        var containerMiddle = _itemsContainerRectTransform.position.y;
        var itemMiddle = SelectedGameObject.GetComponent<RectTransform>().position.y;

        var result = containerMiddle - itemMiddle;
        result = Mathf.Clamp(result, 0, diff);
        return result;
    }

    public void ClearItems()
    {
        _menuItems.Clear();

        foreach (var gameObj in MenuItemsContainer.GetChildren())
        {
            DestroyImmediate(gameObj);
        }

        ForceLayoutUpdate();
        SelectedIndex = 0;
        
    }

    public void AddItem(GameObject newObject)
    {
        var menuItem = newObject.GetComponent<MenuItem>();

        if (menuItem == null)
        {
            throw new ArgumentException("Only GameObjects that include a MenuItem component can be added to Menus.");
        }

        
        newObject.transform.SetParent(MenuItemsContainer.transform);
        var rt = newObject.GetComponent<RectTransform>();
        if (rt != null)
        {
            var posY = MenuItemsBottom.y - (rt.sizeDelta.y / 2);
            newObject.transform.localPosition = new Vector3(0,posY,0);
        }

        _menuItems.Add(menuItem);
        ForceLayoutUpdate();

        if (SelectedIndex > _menuItems.Count)
        {
            SelectedIndex = 0;
        }

        if (_menuItems.Count == 1)
        {
            SelectionHighlight.HighlightObject(newObject);
        }
    }

    public void HandleInput(InputEvent inputEvent)
    {
        if (!this.Enabled)
        {
            return;
        }

        if (this.Player != 0 && this.Player != inputEvent.Player)
        {
            return;
        }

        if (!inputEvent.IsPressed)
        {
            return;
        }

        switch (inputEvent.Action)
        {
            case "Up":
                ChangeSelection(-1);
                break;
            case "Down":
                ChangeSelection(1);
                break;
            case "Left":
                RaiseItemShifted(-1);
                break;
            case "Right":
                RaiseItemShifted(1);
                break;
            case "Pause":
            case "A":
                RaiseItemSelected(false);
                break;
            case "B":
            case "Back":
                RaiseItemSelected(true);
                break;
            
        }

//        MoveHighlight();
    }

    private void RaiseItemShifted(int delta)
    {
        if (SelectedMenuItem == null || !SelectedMenuItem.IsShiftable)
        {
            return;
        }

        SfxSelectionShifted.Play();
        this.SendMessageUpwards("MenuItemShifted", GetEventArgs(SelectedText, delta), SendMessageOptions.DontRequireReceiver);
    }

    private void ChangeSelection(int delta)
    {
        SfxSelectionChanged.Play();
        if (WrapSelection)
        {
            SelectedIndex = Helpers.Wrap(SelectedIndex + delta, _menuItems.Count - 1);
        }
        else
        {
            SelectedIndex = Helpers.Clamp(SelectedIndex + delta, _menuItems.Count - 1);
        }
    }

    private void RaiseItemSelected(bool cancel)
    {
        if (SelectedMenuItem?.Text == CancelMenuAction)
        {
            cancel = true;
        }

        if (cancel)
        {
            RaiseItemCancelled();
            return;
        }

        if (SelectedMenuItem == null || !SelectedMenuItem.IsSelectable)
        {
            return;
        }

        SfxSelectionConfirmed.PlayUnlessNull();
        this.SendMessageUpwards("MenuItemSelected", GetEventArgs(SelectedText));
    }

    private void RaiseItemCancelled()
    {

        // Check if CancelMenuAction is set. If not, this menu cannot be cancelled out of.
        if (string.IsNullOrEmpty(CancelMenuAction))
        {
            return;
        }

        SfxSelectionCancelled.PlayUnlessNull();
        this.SendMessageUpwards("MenuItemSelected", GetEventArgs(CancelMenuAction));
    }

    private MenuEventArgs GetEventArgs(string selectedItem, int shiftAmount = 0)
    {
        return new MenuEventArgs
        {
            Player = this.Player,
            SelectedIndex = this.SelectedIndex,
            SelectedItem = selectedItem,
            ShiftAmount = shiftAmount
        };
    }

    public void RefreshHighlight()
    {
        MoveHighlight();
    }

    public void PlaySound(string sfxName)
    {
        switch (sfxName)
        {
            case "SelectionConfirmed":
                SfxSelectionConfirmed.PlayUnlessNull();
                break;
            case "SelectionCancelled":
                SfxSelectionCancelled.PlayUnlessNull();
                break;
            case "SelectionChanged":
                SfxSelectionChanged.PlayUnlessNull();
                break;
            case "SelectionShifted":
                SfxSelectionShifted.PlayUnlessNull();
                break;
        }
    }
}
