using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : ScreenManager
{
    public float ScrollSpeed = 2.0f;
    public float ScrollSpeedMultiplier = 1.0f;
    public RectTransform CreditsContainer;
    public GameObject RhythmistContainer;
    public Text LeadRhythmistName;

    private int _scrollLimit = 9999;    // Calculated at runtime.

    [Header("Sounds")]
    public AudioSource SfxBack;

    void Awake()
    {
        FindCoreManager();
        LayoutRebuilder.ForceRebuildLayoutImmediate(CreditsContainer);
    }

    private void Start()
    {
        var leadRhythmist = CoreManager.ProfileManager.GetLeadRhythmist();

        if (leadRhythmist == null)
        {
            RhythmistContainer.SetActive(false);
            return;
        }

        RhythmistContainer.SetActive(true);
        LeadRhythmistName.text = leadRhythmist.Name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(CreditsContainer);
        _scrollLimit = (int)CreditsContainer.sizeDelta.y + 100;
    }

    void FixedUpdate()
    {
       Scroll();
    }

    private void Scroll()
    {
        var pos = CreditsContainer.localPosition;
        var newY = pos.y + (ScrollSpeed * ScrollSpeedMultiplier);


        if (newY > _scrollLimit)
        {
            newY = CreditsContainer.sizeDelta.y * -0.5f;
        }

        CreditsContainer.localPosition = new Vector3(pos.x, newY, pos.z);
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.B:
            case InputAction.Back:
                SfxBack.PlayUnlessNull();
                SceneTransition(GameScene.Options);
                break;
            case InputAction.X:
                ScrollSpeedMultiplier = ScrollSpeedMultiplier == 0.0f ? 1.0f : 0.0f;
                break;
            case InputAction.Y:
                ScrollSpeedMultiplier = ScrollSpeedMultiplier == 6.0f ? 1.0f : 6.0f;
                break;
        }
    }

}
