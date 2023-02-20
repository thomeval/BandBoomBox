using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : ScreenManager
{
    public float ScrollSpeed = 2.0f;

    public RectTransform CreditsContainer;
    public GameObject RhythmistContainer;
    public Text LeadRhythmistName;
    public int ScrollLimit = 2600;

    [Header("Sounds")] 
    public AudioSource SfxBack;

    void Awake()
    {
        FindCoreManager();
        LayoutRebuilder.ForceRebuildLayoutImmediate(CreditsContainer);
    }

    private void Start()
    {
        var pos = CreditsContainer.localPosition;

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
        var pos = CreditsContainer.localPosition;
        var newY = pos.y + ScrollSpeed;


        if (newY > ScrollLimit)
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
        }
    }

}
