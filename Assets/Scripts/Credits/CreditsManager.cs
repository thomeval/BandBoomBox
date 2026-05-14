using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class CreditsManager : ScreenManager
{
    public float ScrollSpeed = 2.0f;
    public float ScrollSpeedMultiplier = 1.0f;
    public RectTransform CreditsContainer;
    public GameObject RhythmistContainer;
    public Text LeadRhythmistName;

    private int _scrollLimit = 9999;    // Calculated at runtime.

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
    }

    void FixedUpdate()
    {
       Scroll();
    }

    private void Scroll()
    {
        // Cannot be set during Awake() or Start() due to a bug with ContentSizeFitter causing the calculated height to be initially incorrect.
        _scrollLimit = (int)CreditsContainer.rect.height -960;
        var pos = CreditsContainer.localPosition;
        var newY = pos.y + (ScrollSpeed * ScrollSpeedMultiplier);


        if (newY > _scrollLimit)
        {
            newY = CreditsContainer.rect.height * -0.5f;
        }

        CreditsContainer.localPosition = new Vector3(pos.x, newY, pos.z);
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.B:
            case InputAction.Back:
                PlaySfx(SoundEvent.SelectionCancelled);
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
